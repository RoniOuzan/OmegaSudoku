using System;
using System.Collections.Generic;
using System.Numerics;

namespace OmegaSudoku.Core;

public static class Solver
{
    public static bool Solve(int[,] board)
    {
        int size = board.GetLength(0);
        int boxSize = (int)Math.Sqrt(size);

        int[] rowUsed = new int[size];
        int[] colUsed = new int[size];
        int[] boxUsed = new int[size];
        int allMask = (1 << size) - 1;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int cell = board[i, j];
                if (cell == 0) continue;
                
                int bit = 1 << (cell - 1);
                rowUsed[i] |= bit;
                colUsed[j] |= bit;
                boxUsed[BoxIndex(i, j, boxSize)] |= bit;
            }
        }

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

        return SolveRecursive(board, rowUsed, colUsed, boxUsed, possibilities, emptyCells, new Stack<(int r, int c, int oldMask, bool bitSet)>());
    }

    private static bool SolveRecursive(
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
        
        int boxSize = (int) Math.Sqrt(board.GetLength(0));
        int boxIndex = BoxIndex(r, c, boxSize);

        while (options != 0)
        {
            int bit = options & -options;
            options ^= bit;
            
            int num = BitOperations.TrailingZeroCount(bit) + 1;
            
            int checkpoint = changes.Count;
            
            // Place the number on the cell
            board[r, c] = num;
            rowUsed[r] |= bit;
            colUsed[c] |= bit;
            boxUsed[boxIndex] |= bit;
            
            // Save and clear this cell's possibilities
            changes.Push((r, c, possibilities[r, c], true));
            possibilities[r, c] = 0;

            // Propagate and Update Neighbors Uses
            if (UpdateNeighbors(board, possibilities, r, c, rowUsed, colUsed, boxUsed, changes))
            {
                if (SolveRecursive(board, rowUsed, colUsed, boxUsed, possibilities, emptyCells, changes))
                    return true;
            }

            // Backtrack
            while (changes.Count > checkpoint)
            {
                var (oldR, oldC, oldMask, bitSet) = changes.Pop();
                if (bitSet)
                {
                    int cell = board[oldR, oldC];
                    if (cell != 0)
                    {
                        int oldBit = 1 << (cell - 1);
                        rowUsed[oldR] &= ~oldBit;
                        colUsed[oldC] &= ~oldBit;
                        boxUsed[BoxIndex(oldR, oldC, boxSize)] &= ~oldBit;
                    }
                    board[oldR, oldC] = 0;
                }
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
        int maxNearEmptyCells = -1;

        foreach (var (r, c) in emptyCells)
        {
            if (board[r, c] != 0) continue;

            int count = BitOperations.PopCount((uint)possibilities[r, c]);
            if (count == 0)  return (false, -1, -1);
            if (count < minCount)
            {
                minCount = count;
                bestR = r;
                bestC = c;
                maxNearEmptyCells = CountNearEmptyCells(board, r, c);
            
                if (count == 1) 
                    return (true, r, c);
            }
            else if (count == minCount)
            {
                int nearEmptyCells = CountNearEmptyCells(board, r, c);
                if (nearEmptyCells > maxNearEmptyCells)
                {
                    maxNearEmptyCells = nearEmptyCells;
                    bestR = r;
                    bestC = c;
                }
            }
        }

        return (true, bestR, bestC);
    }

    private static int CountNearEmptyCells(int[,] board, int r, int c)
    {
        int size = board.GetLength(0);
        int boxSize = (int)Math.Sqrt(size);
        int count = 0;

        // Count the empty cells in rows and columns
        for (int i = 0; i < size; i++)
        {
            // Row
            if (i != c &&board[r, i] == 0) count++;
            // Col
            if (i != r &&board[i, c] == 0) count++;
        }

        // Count empty cells in box (avoid double counting row/col)
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

    private static bool UpdateNeighbors(
        int[,] board, 
        int[,] possibilities, 
        int r, 
        int c, 
        int[] rowUsed, 
        int[] colUsed, 
        int[] boxUsed, 
        Stack<(int r, int c, int oldMask, bool bitSet)> changes
    ) {
        int size = board.GetLength(0);
        int boxSize = (int)Math.Sqrt(size);
        
        Queue<(int r, int c)> queue = new Queue<(int r, int c)>();
        queue.Enqueue((r, c));

        while (queue.Count > 0)
        {
            var (currentR, currentC) = queue.Dequeue();
            int cell = board[currentR, currentC];
            int bit = 1 << (cell - 1);

            for (int i = 0; i < size; i++)
            {
                // Row
                if (i != currentC && 
                    !ProcessNeighbor(currentR, i, bit, board, possibilities, rowUsed, colUsed, boxUsed, changes, queue, boxSize)) 
                    return false;
                
                // Col
                if (i != currentR && 
                    !ProcessNeighbor(i, currentC, bit, board, possibilities, rowUsed, colUsed, boxUsed, changes, queue, boxSize)) 
                    return false;
            }

            // Box
            int boxR = (currentR / boxSize) * boxSize;
            int boxC = (currentC / boxSize) * boxSize;
            for (int i = boxR; i < boxR + boxSize; i++)
            {
                if (i == currentR) continue;
                for (int j = boxC; j < boxC + boxSize; j++)
                {
                    if (j == currentC) continue;
                    if (!ProcessNeighbor(i, j, bit, board, possibilities, rowUsed, colUsed, boxUsed, changes, queue, boxSize)) 
                        return false;
                }
            }
        }

        return true;
    }

    private static bool ProcessNeighbor(
        int r, 
        int c, 
        int bit, 
        int[,] board, 
        int[,] possibilities, 
        int[] rowUsed, 
        int[] colUsed, 
        int[] boxUsed, 
        Stack<(int r, int c, int oldMask, bool bitSet)> changes, 
        Queue<(int r, int c)> queue, 
        int boxSize
    ) {
        if (board[r, c] != 0) return true;

        if ((possibilities[r, c] & bit) != 0)
        {
            // update the neighbors
            changes.Push((r, c, possibilities[r, c], false));
            possibilities[r, c] &= ~bit;

            // check for propagation
            int count = BitOperations.PopCount((uint)possibilities[r, c]);
            if (count == 0) return false;

            if (count == 1)
            {
                int nextNum = BitOperations.TrailingZeroCount(possibilities[r, c]) + 1;
                
                int nextBit = 1 << (nextNum - 1);
                // check if this move is actually valid
                if ((rowUsed[r] & nextBit) != 0 || (colUsed[c] & nextBit) != 0 || (boxUsed[BoxIndex(r, c, boxSize)] & nextBit) != 0)
                    return false;

                var last = changes.Pop();
                changes.Push((last.r, last.c, last.oldMask, true));

                board[r, c] = nextNum;
                rowUsed[r] |= nextBit;
                colUsed[c] |= nextBit;
                boxUsed[BoxIndex(r, c, boxSize)] |= nextBit;
                
                possibilities[r, c] = 0;
                queue.Enqueue((r, c));
            }
        }
        return true;
    }

    private static int BoxIndex(int row, int col, int boxSize)
    {
        return (row / boxSize) * boxSize + (col / boxSize);
    }
}