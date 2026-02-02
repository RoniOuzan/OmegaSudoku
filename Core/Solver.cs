using System.Collections.Generic;
using System.Numerics;

namespace OmegaSudoku.Core;

public static class Solver
{
    public static bool Solve(Board board)
    {
        int size = board.Size;

        int[] rowUsed = new int[size];
        int[] colUsed = new int[size];
        int[] boxUsed = new int[size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Cell cell = board.GetCell(i, j);
                if (cell.IsEmpty) continue;

                int bit = 1 << cell.Number;
                rowUsed[i] |= bit;
                colUsed[j] |= bit;
                boxUsed[BoxIndex(i, j, board.BoxSize)] |= bit;
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
                Cell cell = board.GetCell(r, c);
                if (cell.IsSolved) continue;

                int used = rowUsed[r] | colUsed[c] | boxUsed[BoxIndex(r, c, board.BoxSize)];
                possibilities[r, c] = allMask & ~used;

                emptyCells.Add((r, c));
            }
        }

        return SolveRecursive(board, rowUsed, colUsed, boxUsed, possibilities, emptyCells, new Stack<(int r, int c, int oldMask)>());
    }

    private static bool SolveRecursive(
        Board board, 
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
        int boxIndex = BoxIndex(r, c, board.BoxSize);

        // No options lefts
        while (options != 0)
        {
            int bit = options & -options;
            options ^= bit;
            
            int num = BitOperations.TrailingZeroCount(bit);
            
            int checkpoint = changes.Count;
            
            // Place the number on the cell
            board.GetCell(r, c).Number = num;
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
            board.GetCell(r, c).Number = 0;
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

    private static (bool, int, int) FindBestEmptyCell(Board board, int[,] possibilities, List<(int r, int c)> emptyCells)
    {
        int bestR = -1;
        int bestC = -1;
        int minCount = int.MaxValue;

        foreach (var (r, c) in emptyCells)
        {
            if (!board.GetCell(r, c).IsEmpty)
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
        Board board, 
        int[,] possibilities, 
        int currentR, 
        int currentC, 
        int bit, 
        Stack<(int r, int c, int oldMask)> changes
    ) {
        // Row
        for (int c = 0; c < board.Size; c++)
        {
            if (board.GetCell(currentR, c).IsSolved) continue;
            
            if ((possibilities[currentR, c] & bit) != 0)
            {
                changes.Push((currentR, c, possibilities[currentR, c]));
                possibilities[currentR, c] &= ~bit;
            }
        }

        // Column
        for (int r = 0; r < board.Size; r++)
        {
            if (board.GetCell(r, currentC).IsSolved) continue;
            
            if ((possibilities[r, currentC] & bit) != 0)
            {
                changes.Push((r, currentC, possibilities[r, currentC]));
                possibilities[r, currentC] &= ~bit;
            }
        }

        // Box
        int boxSize = board.BoxSize;
        int startRow = (currentR / boxSize) * boxSize;
        int startCol = (currentC / boxSize) * boxSize;
        for (int r = startRow; r < startRow + boxSize; r++)
        {
            for (int c = startCol; c < startCol + boxSize; c++)
            {
                if (board.GetCell(r, c).IsSolved) continue;
                
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