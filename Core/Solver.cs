using System;
using System.Collections.Generic;
using System.Numerics;

namespace OmegaSudoku.Core;

public static class Solver
{
    public static bool Solve(int[,] board)
    {
        int[] rowUsed = new int[Board.Size];
        int[] colUsed = new int[Board.Size];
        int[] boxUsed = new int[Board.Size];
        int allMask = (1 << Board.Size) - 1;

        for (int i = 0; i < Board.Size; i++)
        {
            for (int j = 0; j < Board.Size; j++)
            {
                int cell = board[i, j];
                if (cell == 0) continue;
                
                int bit = 1 << (cell - 1);
                rowUsed[i] |= bit;
                colUsed[j] |= bit;
                boxUsed[BoxIndex(i, j)] |= bit;
            }
        }

        int[,] possibilities = new int[Board.Size, Board.Size];
        List<(int r, int c)> emptyCells = new List<(int r, int c)>();
        for (int r = 0; r < Board.Size; r++)
        {
            for (int c = 0; c < Board.Size; c++)
            {
                int cell = board[r, c];
                if (cell != 0) continue;

                int used = rowUsed[r] | colUsed[c] | boxUsed[BoxIndex(r, c)];
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
        
        int boxIndex = BoxIndex(r, c);

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
                        boxUsed[BoxIndex(oldR, oldC)] &= ~oldBit;
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
        Queue<(int r, int c)> queue = new Queue<(int r, int c)>();
        queue.Enqueue((r, c));

        while (queue.Count > 0)
        {
            var (currentR, currentC) = queue.Dequeue();
            int cell = board[currentR, currentC];
            int bit = 1 << (cell - 1);

            for (int i = 0; i < Board.Size; i++)
            {
                // Row
                if (i != currentC && 
                    !ProcessNeighbor(currentR, i, bit, board, possibilities, rowUsed, colUsed, boxUsed, changes, queue)) 
                    return false;
                
                // Col
                if (i != currentR && 
                    !ProcessNeighbor(i, currentC, bit, board, possibilities, rowUsed, colUsed, boxUsed, changes, queue)) 
                    return false;
            }

            // Box
            int boxR = (currentR / Board.BoxSize) * Board.BoxSize;
            int boxC = (currentC / Board.BoxSize) * Board.BoxSize;
            for (int i = boxR; i < boxR + Board.BoxSize; i++)
            {
                if (i == currentR) continue;
                for (int j = boxC; j < boxC + Board.BoxSize; j++)
                {
                    if (j == currentC) continue;
                    if (!ProcessNeighbor(i, j, bit, board, possibilities, rowUsed, colUsed, boxUsed, changes, queue)) 
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
        Queue<(int r, int c)> queue
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
                if ((rowUsed[r] & nextBit) != 0 || (colUsed[c] & nextBit) != 0 || (boxUsed[BoxIndex(r, c)] & nextBit) != 0)
                    return false;

                var last = changes.Pop();
                changes.Push((last.r, last.c, last.oldMask, true));

                board[r, c] = nextNum;
                rowUsed[r] |= nextBit;
                colUsed[c] |= nextBit;
                boxUsed[BoxIndex(r, c)] |= nextBit;
                
                possibilities[r, c] = 0;
                queue.Enqueue((r, c));
            }
        }
        return true;
    }

    private static int BoxIndex(int row, int col)
    {
        return (row / Board.BoxSize) * Board.BoxSize + (col / Board.BoxSize);
    }
}