using System;
using System.Collections.Generic;
using System.Numerics;

namespace OmegaSudoku.Core;

public static class Solver
{
    public static bool Solve(int[,] board)
    {
        int size = board.GetLength(0);
        int boxSize = (int) Math.Sqrt(size);

        int[] rowUsed = new int[size];
        int[] colUsed = new int[size];
        int[] boxUsed = new int[size];
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int cell = board[i, j];
                if (cell == 0) continue;

                int bit = 1 << cell;
                rowUsed[i] |= bit;
                colUsed[j] |= bit;
                boxUsed[BoxIndex(i, j, boxSize)] |= bit;
            }
        }
        
        int allMask = 0;
        for (int n = 1; n <= size; n++)
            allMask |= 1 << n;

        int[,] possibilities = new int[size, size];
        List<(int r, int c)> emptyCells = new List<(int r, int c)>();
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                int cell = board[r, c];
                if (cell != 0) continue;

                int used = rowUsed[r] | colUsed[c] | boxUsed[BoxIndex(r, c, boxSize)];
                possibilities[r, c] = allMask & ~used;

                emptyCells.Add((r, c));
            }
        }

        return SolveRecursive(board, rowUsed, colUsed, boxUsed, possibilities, emptyCells, new Stack<(int r, int c, int oldMask)>());
    }

    private static bool SolveRecursive(
        int[,] board, 
        int[] rowUsed, 
        int[] colUsed, 
        int[] boxUsed,
        int[,] possibilities,
        List<(int r, int c)> emptyCells,
        Stack<(int r, int c, int oldMask)> changes
    ) {
        var (possible, r, c) = FindBestEmptyCell(board, possibilities, emptyCells);
        if (!possible) return false; // dead end because the amount of possibilities is 0
        if (r == -1) return true; // No empty cells left, so the board is solved
        
        int options = possibilities[r, c];
        
        int boxSize = (int) Math.Sqrt(board.GetLength(0));
        int boxIndex = BoxIndex(r, c, boxSize);

        // No options lefts
        while (options != 0)
        {
            int bit = options & -options;
            options ^= bit;
            
            int num = BitOperations.TrailingZeroCount(bit);
            
            int checkpoint = changes.Count;
            
            // Place the number on the cell
            board[r, c] = num;
            rowUsed[r] |= bit;
            colUsed[c] |= bit;
            boxUsed[boxIndex] |= bit;
            
            // Save and clear this cell's possibilities
            changes.Push((r, c, possibilities[r, c]));
            possibilities[r, c] = 0;
            
            UpdateNeighbors(board, possibilities, r, c, bit, changes);
            
            if (SolveRecursive(board, rowUsed, colUsed, boxUsed,
                    possibilities, emptyCells, changes))
                return true;
            
            // Backtrack
            board[r, c] = 0;
            rowUsed[r] &= ~bit;
            colUsed[c] &= ~bit;
            boxUsed[boxIndex] &= ~bit;

            while (changes.Count > checkpoint)
            {
                var (oldR, oldC, oldMask) = changes.Pop();
                possibilities[oldR, oldC] = oldMask;
            }
        }

        return false;
    }

    private static (bool, int, int) FindBestEmptyCell(int[,] board, int[,] possibilities, List<(int r, int c)> emptyCells)
    {
        int bestR = -1;
        int bestC = -1;
        int minCount = int.MaxValue;

        foreach (var (r, c) in emptyCells)
        {
            if (board[r, c] != 0)
                continue;

            int mask = possibilities[r, c];
            int count = BitOperations.PopCount((uint)mask);

            if (count == 0) 
                return (false, -1, -1);
            
            if (count < minCount)
            {
                minCount = count;
                bestR = r;
                bestC = c;
                
                if (count == 1) 
                    return (true, bestR, bestC);
            }
        }

        return (true, bestR, bestC);
    }

    private static void UpdateNeighbors(
        int[,] board, 
        int[,] possibilities, 
        int currentR, 
        int currentC, 
        int bit, 
        Stack<(int r, int c, int oldMask)> changes
    )
    {
        int size = board.GetLength(0);
        int boxSize = (int)Math.Sqrt(size);
        
        // Row
        for (int c = 0; c < size; c++)
        {
            if (board[currentR, c] != 0) continue;
            
            if ((possibilities[currentR, c] & bit) != 0)
            {
                changes.Push((currentR, c, possibilities[currentR, c]));
                possibilities[currentR, c] &= ~bit;
            }
        }

        // Column
        for (int r = 0; r < size; r++)
        {
            if (board[r, currentC] != 0) continue;
            
            if ((possibilities[r, currentC] & bit) != 0)
            {
                changes.Push((r, currentC, possibilities[r, currentC]));
                possibilities[r, currentC] &= ~bit;
            }
        }

        // Box
        int startRow = (currentR / boxSize) * boxSize;
        int startCol = (currentC / boxSize) * boxSize;
        for (int r = startRow; r < startRow + boxSize; r++)
        {
            for (int c = startCol; c < startCol + boxSize; c++)
            {
                if (board[r, c] != 0) continue;
                
                if ((possibilities[r, c] & bit) != 0)
                {
                    changes.Push((r, c, possibilities[r, c]));
                    possibilities[r, c] &= ~bit;
                }
            }
        }
    }
    
    private static int BoxIndex(int row, int column, int boxSize)
    {
        return (row / boxSize) * boxSize + (column / boxSize);
    }
}