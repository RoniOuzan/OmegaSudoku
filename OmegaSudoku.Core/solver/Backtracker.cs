using System.Numerics;

namespace OmegaSudoku.Core;

public static class Backtracker
{
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
    /// Recursively solves the board using backtracking.
    /// </summary>
    public static bool SolveRecursive(
        int[,] board, 
        int[] rowUsed, 
        int[] colUsed, 
        int[] boxUsed,
        int[,] possibilities,
        List<(int r, int c)> emptyCells,
        Stack<(int r, int c, int oldMask, bool bitSet)> changes
    ) {
        var (possible, r, c) = FindBestEmptyCell(board, possibilities, emptyCells);
        if (!possible) return false; // dead end because the amount of possibilities is 0
        if (r == -1) return true; // No empty cells left, so the board is solved
        
        int options = possibilities[r, c];
        
        int boxIndex = Solver.BoxLookup[r, c];

        while (options != 0)
        {
            int bit = options & -options;
            options ^= bit;
            
            int num = BitOperations.TrailingZeroCount(bit) + 1; // +1 because index 0 is the first number
            
            int checkpoint = changes.Count; // for the backtrack after
            
            // Place the number on the cell
            board[r, c] = num;
            rowUsed[r] |= bit;
            colUsed[c] |= bit;
            boxUsed[boxIndex] |= bit;
            
            // Save and clear this cell's possibilities
            changes.Push((r, c, possibilities[r, c], true));
            possibilities[r, c] = 0;

            // Propagate and Update Neighbors Uses
            if (Propagation.UpdateNeighbors(board, possibilities, r, c, rowUsed, colUsed, boxUsed, changes))
            {
                if (SolveRecursive(board, rowUsed, colUsed, boxUsed, possibilities, emptyCells, changes))
                    return true;
            }

            // Backtrack
            while (changes.Count > checkpoint)
            {
                var (oldR, oldC, oldMask, bitSet) = changes.Pop();
                // Restore the bit
                if (bitSet)
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
                possibilities[oldR, oldC] = oldMask;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Chooses the best empty cell to try next using Minimum Remaining Values (MRV) and tie-breaking heuristics.
    /// </summary>
    /// <returns>if the current state if the board is possible to solve, the row and the column of the best empty cell</returns>
    private static (bool possible, int row, int col) FindBestEmptyCell(int[,] board, int[,] possibilities, List<(int r, int c)> emptyCells)
    {
        int bestR = -1;
        int bestC = -1;
        int minCount = 10;
        
        int maxConnectivity = -1;
        int maxNear = -1;
        
        foreach (var (r, c) in emptyCells)
        {
            if (board[r, c] != 0) continue;
            
            int count = BitOperations.PopCount((uint)possibilities[r, c]);
            if (count == 0) return (false, -1, -1);
            if (count == 1) return (true, r, c);
            if (count < minCount)
            {
                minCount = count; 
                bestR = r; 
                bestC = c; 
                
                maxConnectivity = ConnectivityMap[r, c];
                maxNear = -1; // call CountNearEmptyCells only when you have to
            }
            else if (count == minCount)
            {
                if (maxNear == -1) maxNear = CountNearEmptyCells(board, bestR, bestC);
             
                // Tie-breaker: Pick the cell with the highest nearby empty cells
                int near = CountNearEmptyCells(board, r, c);
                if (near > maxNear)
                {
                    maxNear = near;
                    bestR = r;
                    bestC = c;
                }
                else if (near == maxNear)
                {
                    // Tie-breaker: Pick the cell with the highest connectivity to other constraints
                    int currentConnectivity = ConnectivityMap[r, c];
                    if (currentConnectivity > maxConnectivity)
                    {
                        maxConnectivity = currentConnectivity;
                        bestR = r;
                        bestC = c;
                    } 
                }
            }
        }
        return (true, bestR, bestC);
    }

    /// <summary>
    /// Counts the number of empty neighboring cells (row, column, and box) of a given cell.
    /// </summary>
    private static int CountNearEmptyCells(int[,] board, int r, int c)
    {
        int count = 0;

        // Count the empty cells in rows and columns
        for (int i = 0; i < Board.Size; i++)
        {
            // Row
            if (i != c &&board[r, i] == 0) count++;
            // Col
            if (i != r &&board[i, c] == 0) count++;
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