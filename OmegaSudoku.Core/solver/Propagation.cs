using System.Numerics;

namespace OmegaSudoku.Core;

public static class Propagation
{
    /// <summary>
    /// Propagates constraints to neighboring cells when a number is placed.
    /// </summary>
    public static bool UpdateNeighbors(
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

    /// <summary>
    /// Processes a neighbor cell during propagation, updating its possibilities.
    /// </summary>
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

        if ((possibilities[r, c] & bit) == 0) return true;
        
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
            if ((rowUsed[r] & nextBit) != 0 || (colUsed[c] & nextBit) != 0 || (boxUsed[Solver.BoxLookup[r, c]] & nextBit) != 0)
                return false;

            var last = changes.Pop();
            changes.Push((last.r, last.c, last.oldMask, true));

            board[r, c] = nextNum;
            rowUsed[r] |= nextBit;
            colUsed[c] |= nextBit;
            boxUsed[Solver.BoxLookup[r, c]] |= nextBit;
                
            possibilities[r, c] = 0;
            queue.Enqueue((r, c));
        }
        
        return true;
    }
}