using System.Numerics;

namespace OmegaSudoku.Core;

/// <summary>
/// Implements the recursive search engine for Sudoku solving.
/// 
/// Uses:
/// - Backtracking
/// - Bitmask constraints
/// - Constraint propagation
/// - MRV (Minimum Remaining Values) with tie-breaking heuristics
/// </summary>
public static class Backtracker
{
    /// Precomputed connectivity map used for tie-breaking when selecting the next empty cell.
    /// Higher values indicate more connected cells that are more constrained.
    private static readonly int[,] ConnectivityMap =
    {
        { 2, 2, 2, 2, 2, 2, 2, 2, 2 },
        { 2, 3, 3, 2, 3, 3, 2, 3, 3 },
        { 2, 3, 3, 2, 3, 3, 2, 3, 3 },
        { 2, 2, 2, 4, 4, 4, 2, 2, 2 },
        { 2, 3, 3, 4, 5, 4, 3, 3, 2 },
        { 2, 2, 2, 4, 4, 4, 2, 2, 2 },
        { 2, 3, 3, 2, 3, 3, 2, 3, 3 },
        { 2, 3, 3, 2, 3, 3, 2, 3, 3 },
        { 2, 2, 2, 2, 2, 2, 2, 2, 2 }
    };

    /// <summary>
    /// Recursively solves a Sudoku board using backtracking, bitmask, propagation and MRV.
    /// </summary>
    /// <returns><c>true</c> if the board was successfully solved; otherwise, <c>false</c>.</returns>
    public static bool SolveRecursive(SolverState state) {
        var (possible, r, c) = FindBestEmptyCell(state);
        if (!possible) return false; // dead end because cell has no possibilities
        if (r == -1) return true;    // No empty cells left, so the board is solved
        
        int options = state.AvailableNumbers[r, c];

        while (options != 0)
        {
            // Get the lowest set bit
            int bit = options & -options;
            options ^= bit;
            int num = BitOperations.TrailingZeroCount(bit) + 1; // +1 because index 0 is the first number
            
            int checkpoint = state.Changes.Count; // to backtrack after
            PlaceNumber(state, r, c, num, bit);

            // Propagate and Update Neighbors Uses
            if (Propagation.UpdateNeighbors(state, r, c))
            {
                if (SolveRecursive(state))
                    return true;
            }

            // Backtrack: undo changes made after checkpoint
            RollbackChanges(state, checkpoint);
        }
        return false;
    }

    /// <summary>
    /// Places a number in a cell and updates all constraint masks.
    /// The previous state is recorded for backtracking.
    /// </summary>
    private static void PlaceNumber(SolverState state, int row, int col, int num, int bit) {
        // Place the number on the cell
        state.Board[row, col] = num;
        state.RowUsed[row] |= bit;
        state.ColUsed[col] |= bit;
        state.BoxUsed[state.BoxLookup[row, col]] |= bit;

        // Save and clear this cell's possibilities
        state.Changes.Push(new BoardChange(row, col, state.AvailableNumbers[row, col], true));
        state.AvailableNumbers[row, col] = 0;
    }

    /// <summary>
    /// Reverts all changes performed after the specified checkpoint.
    /// Restores board values, masks, and possibility states.
    /// </summary>
    private static void RollbackChanges(SolverState state, int checkpoint)
    {
        while (state.Changes.Count > checkpoint)
        {
            var change = state.Changes.Pop();
            var oldR = change.Row;
            var oldC = change.Col;
            
            // Restore the bit
            if (change.BitSet)
            {
                int cell = state.Board[oldR, oldC];
                if (cell != 0)
                {
                    int oldBit = 1 << (cell - 1);
                    state.RowUsed[oldR] &= ~oldBit;
                    state.ColUsed[oldC] &= ~oldBit;
                    state.BoxUsed[state.BoxLookup[oldR, oldC]] &= ~oldBit;
                }
                state.Board[oldR, oldC] = 0;
            }
            
            state.AvailableNumbers[oldR, oldC] = change.OldMask;
        }
    }
    
    /// <summary>
    /// Selects the next empty cell using:
    /// 1. MRV (fewest remaining possibilities)
    /// 2. Number of empty neighbors
    /// 3. Connectivity weight
    /// </summary>
    /// <returns>
    /// A tuple of:
    /// <list type="bullet">
    /// <item><c>possible</c>: <c>false</c> if a cell has zero possibilities (unsolvable state).</item>
    /// <item><c>row</c>: Row index of the selected cell.</item>
    /// <item><c>col</c>: Column index of the selected cell.</item>
    /// </list>
    /// </returns>
    private static (bool possible, int row, int col) FindBestEmptyCell(SolverState state)
    {
        int bestR = -1;
        int bestC = -1;
        int minPossibilities = 10;
        
        int bestConnectivity = -1;
        int bestNeighbors = -1;
        
        foreach (Cell cell in state.EmptyCells)
        {
            int r = cell.Row;
            int c = cell.Col;
            if (state.Board[r, c] != 0) continue;
            
            // Gets the amount of possibilities
            int cellPossibilities = BitOperations.PopCount((uint)state.AvailableNumbers[r, c]);
            if (cellPossibilities == 0) return (false, -1, -1);
            if (cellPossibilities == 1) return (true, r, c);

            if (CompareCell(state, cellPossibilities, r, c, minPossibilities, bestNeighbors, bestConnectivity))
            {
                bestR = r;
                bestC = c;
                minPossibilities = cellPossibilities;
                bestNeighbors = CountEmptyCellsNeighbors(state, r, c);
                bestConnectivity = ConnectivityMap[r, c];
            }
        }
        return (true, bestR, bestC);
    }
        
    /// <summary>
    /// Determines whether the current cell is a better candidate
    /// than the current best according to MRV and tie-break rules.
    /// </summary>
    private static bool CompareCell(SolverState state, int cellPossibilities, int r, int c, int minPossibilities, int bestNeighbors, int bestConnectivity)
    {
        // If it has fewer possibilities it's better
        if (cellPossibilities < minPossibilities)
            return true;

        // Tie-Breaker for empty neighbors and then connectivity
        if (cellPossibilities == minPossibilities)
        {
            int neighbors = CountEmptyCellsNeighbors(state, r, c);
            int connectivity = ConnectivityMap[r, c];

            // If it has more neighbors, if they are the same -> compare connectivity
            return neighbors > bestNeighbors ||
                   (neighbors == bestNeighbors && connectivity > bestConnectivity);
        }
        return false;
    }

    /// <summary>
    /// Counts empty cells in the same row, column, and box.
    /// Used as a secondary heuristic for tie-breaking.
    /// </summary>
    /// <returns>The count of empty neighboring cells.</returns>
    private static int CountEmptyCellsNeighbors(SolverState state, int r, int c)
    {
        var board = state.Board;
        int count = 0;

        // Count the empty cells in rows and columns
        for (int i = 0; i < state.Size; i++)
        {
            // Row
            if (i != c && board[r, i] == 0) count++;
            // Col
            if (i != r && board[i, c] == 0) count++;
        }

        // Count empty cells in box (avoid double counting row/col)
        var boxSize = state.BoxSize;
        int boxR = (r / boxSize) * boxSize;
        int boxC = (c / boxSize) * boxSize;
        for (int i = boxR; i < boxR + boxSize; i++)
        {
            if (i == r) continue;
            for (int j = boxC; j < boxC + boxSize; j++)
            {
                if (j == c) continue;
                
                if (board[i, j] == 0) count++;
            }
        }
        return count;
    }
}