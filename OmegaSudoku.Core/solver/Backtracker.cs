using System.Numerics;

namespace OmegaSudoku.Core;

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
    /// <param name="board">The current Sudoku board. Empty cells are represented by 0.</param>
    /// <param name="rowUsed">Bitmask array representing which numbers are used in each row.</param>
    /// <param name="colUsed">Bitmask array representing which numbers are used in each column.</param>
    /// <param name="boxUsed">Bitmask array representing which numbers are used in each box.</param>
    /// <param name="possibilities">Bitmask array representing possible numbers for each cell.</param>
    /// <param name="emptyCells">List of the empty cells on the board to fill.</param>
    /// <param name="changes">Stack used to track changes for efficient backtracking.</param>
    /// <returns><c>true</c> if the board was successfully solved; otherwise, <c>false</c>.</returns>
    public static bool SolveRecursive(
        int[,] board, 
        int[] rowUsed, 
        int[] colUsed, 
        int[] boxUsed,
        int[,] possibilities,
        List<Cell> emptyCells,
        Stack<BoardChange> changes
    ) {
        var (possible, r, c) = FindBestEmptyCell(board, possibilities, emptyCells);
        if (!possible) return false; // dead end because cell has no possibilities
        if (r == -1) return true; // No empty cells left, so the board is solved
        
        int options = possibilities[r, c];

        while (options != 0)
        {
            // Get the lowest set bit
            int bit = options & -options;
            options ^= bit;
            int num = BitOperations.TrailingZeroCount(bit) + 1; // +1 because index 0 is the first number
            
            int checkpoint = changes.Count; // to backtrack after
            PlaceNumber(board, rowUsed, colUsed, boxUsed, possibilities, changes, r, c, num, bit);

            // Propagate and Update Neighbors Uses
            if (Propagation.UpdateNeighbors(board, rowUsed, colUsed, boxUsed, possibilities, changes, r, c))
            {
                if (SolveRecursive(board, rowUsed, colUsed, boxUsed, possibilities, emptyCells, changes))
                    return true;
            }

            // Backtrack: undo changes made after checkpoint
            RollbackChanges(board, rowUsed, colUsed, boxUsed, possibilities, checkpoint, changes);
        }
        return false;
    }
    
    /// <summary>
    /// Places a number on the board in the given place.
    /// Also updates the bitmask, every possibility and everything.
    /// </summary>
    /// <param name="board">Current state of the Sudoku board.</param>
    /// <param name="rowUsed">Bitmask array representing which numbers are used in each row.</param>
    /// <param name="colUsed">Bitmask array representing which numbers are used in each column.</param>
    /// <param name="boxUsed">Bitmask array representing which numbers are used in each box.</param>
    /// <param name="possibilities">Current bitmask of possible numbers for each cell.</param>
    /// <param name="changes">Stack used to track changes for efficient backtracking.</param>
    /// <param name="row">Row index of the cell.</param>
    /// <param name="col">Column index of the cell.</param>
    /// <param name="num">Number to put on the given cell.</param>
    /// <param name="bit">A bit of the given cell for the bitmask.</param>
    private static void PlaceNumber(
        int[,] board,
        int[] rowUsed,
        int[] colUsed,
        int[] boxUsed,
        int[,] possibilities,
        Stack<BoardChange> changes,
        int row, int col, int num, int bit)
    {
        // Place the number on the cell
        board[row, col] = num;
        rowUsed[row] |= bit;
        colUsed[col] |= bit;
        boxUsed[Solver.BoxLookup[row, col]] |= bit;

        // Save and clear this cell's possibilities
        changes.Push(new BoardChange(row, col, possibilities[row, col], true));
        possibilities[row, col] = 0;
    }

    /// <summary>
    /// Rollbacks all the changes that was made after the checkpoint.
    /// </summary>
    /// <param name="board">Current state of the Sudoku board.</param>
    /// <param name="rowUsed">Bitmask array representing which numbers are used in each row.</param>
    /// <param name="colUsed">Bitmask array representing which numbers are used in each column.</param>
    /// <param name="boxUsed">Bitmask array representing which numbers are used in each box.</param>
    /// <param name="possibilities">Current bitmask of possible numbers for each cell.</param>
    /// <param name="checkpoint"></param>
    /// <param name="changes">Stack used to track changes to rollback them.</param>
    private static void RollbackChanges(
        int[,] board,
        int[] rowUsed, 
        int[] colUsed, 
        int[] boxUsed,
        int[,] possibilities,
        int checkpoint,
        Stack<BoardChange> changes
    )
    {
        while (changes.Count > checkpoint)
        {
            var change = changes.Pop();
            int oldR = change.Row;
            int oldC = change.Col;
            
            // Restore the bit
            if (change.BitSet)
            {
                int cell = board[oldR, oldC];
                if (cell != 0)
                {
                    int oldBit = 1 << (cell - 1);
                    rowUsed[oldR] &= ~oldBit;
                    colUsed[oldC] &= ~oldBit;
                    boxUsed[Solver.BoxLookup[oldR, oldC]] &= ~oldBit;
                }
                board[oldR, oldC] = 0;
            }
            
            possibilities[oldR, oldC] = change.OldMask;
        }
    }
    
    /// <summary>
    /// Selects the next empty cell to attempt using Minimum Remaining Values (MRV) 
    /// and tie-breaking heuristics based on nearby empty cells and connectivity (in order).
    /// </summary>
    /// <param name="board">Current state of the Sudoku board.</param>
    /// <param name="possibilities">Current bitmask of possible numbers for each cell.</param>
    /// <param name="emptyCells">List of empty cells on the board.</param>
    /// <returns>
    /// A tuple of:
    /// <list type="bullet">
    /// <item><c>possible</c>: <c>false</c> if a cell has zero possibilities (unsolvable state).</item>
    /// <item><c>row</c>: Row index of the selected cell.</item>
    /// <item><c>col</c>: Column index of the selected cell.</item>
    /// </list>
    /// </returns>
    private static (bool possible, int row, int col) FindBestEmptyCell(int[,] board, int[,] possibilities, List<Cell> emptyCells)
    {
        int bestR = -1;
        int bestC = -1;
        int minPossibilities = 10;
        
        int bestConnectivity = -1;
        int bestNeighbors = -1;
        
        foreach (Cell cell in emptyCells)
        {
            int r = cell.Row;
            int c = cell.Col;
            if (board[r, c] != 0) continue;
            
            // Gets the amount of possibilities
            int cellPossibilities = BitOperations.PopCount((uint)possibilities[r, c]);
            if (cellPossibilities == 0) return (false, -1, -1);
            if (cellPossibilities == 1) return (true, r, c);

            if (CompareCell(board, cellPossibilities, r, c, minPossibilities, bestNeighbors, bestConnectivity))
            {
                bestR = r;
                bestC = c;
                minPossibilities = cellPossibilities;
                bestNeighbors = CountEmptyCellsNeighbors(board, r, c);
                bestConnectivity = ConnectivityMap[r, c];
            }
        }
        return (true, bestR, bestC);
    }
        
    private static bool CompareCell(int[,] board, int cellPossibilities, int r, int c, int minPossibilities, int bestNeighbors, int bestConnectivity)
    {
        // If it has fewer possibilities it's better
        if (cellPossibilities < minPossibilities)
            return true;
            
        if (cellPossibilities != minPossibilities) return false;
            
        // Tie-Breaker for empty neighbors and then connectivity
        int neighbors = CountEmptyCellsNeighbors(board, r, c);
        int connectivity = ConnectivityMap[r, c];

        // If it has more neighbors, if they are the same -> compare connectivity
        return neighbors > bestNeighbors ||
               (neighbors == bestNeighbors && connectivity > bestConnectivity);
    }

    /// <summary>
    /// Counts the number of empty neighboring cells in the same row, column, and box for a given cell.
    /// </summary>
    /// <param name="board">Current Sudoku board.</param>
    /// <param name="r">Row index of the cell.</param>
    /// <param name="c">Column index of the cell.</param>
    /// <returns>The count of empty neighboring cells.</returns>
    private static int CountEmptyCellsNeighbors(int[,] board, int r, int c)
    {
        int count = 0;

        // Count the empty cells in rows and columns
        for (int i = 0; i < Board.Size; i++)
        {
            // Row
            if (i != c && board[r, i] == 0) count++;
            // Col
            if (i != r && board[i, c] == 0) count++;
        }

        // Count empty cells in box (avoid double counting row/col)
        int boxR = (r / Board.BoxSize) * Board.BoxSize;
        int boxC = (c / Board.BoxSize) * Board.BoxSize;
        for (int i = boxR; i < boxR + Board.BoxSize; i++)
        {
            if (i == r) continue;
            for (int j = boxC; j < boxC + Board.BoxSize; j++)
            {
                if (j == c) continue;
                
                if (board[i, j] == 0) count++;
            }
        }
        return count;
    }
}