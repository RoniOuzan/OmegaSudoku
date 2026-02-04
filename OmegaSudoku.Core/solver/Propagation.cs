using System.Numerics;

namespace OmegaSudoku.Core;

public static class Propagation
{
    /// <summary>
    /// Propagates constraints to neighboring cells when a number is placed.
    /// </summary>
    /// <param name="board">The current Sudoku board. Empty cells are represented by 0.</param>
    /// <param name="rowUsed">Bitmask array representing which numbers are used in each row.</param>
    /// <param name="colUsed">Bitmask array representing which numbers are used in each column.</param>
    /// <param name="boxUsed">Bitmask array representing which numbers are used in each box.</param>
    /// <param name="possibilities">Bitmask array representing possible numbers for each cell.</param>
    /// <param name="changes">Stack used to track changes for efficient backtracking.</param>
    /// <param name="row">Row index of the updated cell.</param>
    /// <param name="col">Column index of the updated cell.</param>
    /// <returns>
    /// <c>true</c> if propagation succeeds without contradictions; 
    /// <c>false</c> if a contradiction is detected (a cell has no valid possibilities).
    /// </returns>
    public static bool UpdateNeighbors(
        int[,] board, 
        int[] rowUsed, 
        int[] colUsed, 
        int[] boxUsed, 
        int[,] possibilities, 
        Stack<(int r, int c, int oldMask, bool bitSet)> changes,
        int row, 
        int col
    ) {
        Queue<(int r, int c)> queue = new Queue<(int r, int c)>();
        queue.Enqueue((row, col));

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
    /// Processes a neighboring cell during propagation by removing an invalid bit
    /// from its possibilities and triggering further propagation if needed.
    /// </summary>
    /// <param name="r">Row index of the neighbor cell.</param>
    /// <param name="c">Column index of the neighbor cell.</param>
    /// <param name="bit">Bit representing the number to eliminate.</param>
    /// <param name="board">Current Sudoku board.</param>
    /// <param name="possibilities">Bitmask of possible values per cell.</param>
    /// <param name="rowUsed">Bitmask array for row constraints.</param>
    /// <param name="colUsed">Bitmask array for column constraints.</param>
    /// <param name="boxUsed">Bitmask array for box constraints.</param>
    /// <param name="changes">Stack used to record changes for rollback.</param>
    /// <param name="queue">Queue of cells to process for further propagation.</param>
    /// <returns>
    /// <c>true</c> if the neighbor remains valid; 
    /// <c>false</c> if a contradiction is found.
    /// </returns>
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
        // Ignore already filled cells
        if (board[r, c] != 0) return true;

        // If this bit is not present, nothing to remove
        if ((possibilities[r, c] & bit) == 0) return true;
        
        // Save previous state before modifying
        changes.Push((r, c, possibilities[r, c], false));
        possibilities[r, c] &= ~bit;
        
        int count = BitOperations.PopCount((uint)possibilities[r, c]);
        // No possibilities left -> contradiction
        if (count == 0) return false;

        // If only one possibility remains, auto-assign
        if (count == 1)
        {
            int nextNum = BitOperations.TrailingZeroCount(possibilities[r, c]) + 1;
            int nextBit = 1 << (nextNum - 1);
            
            // Validate assignment against constraints
            if ((rowUsed[r] & nextBit) != 0 || (colUsed[c] & nextBit) != 0 ||
                (boxUsed[Solver.BoxLookup[r, c]] & nextBit) != 0)
                return false;

            // Mark this change as a committed placement
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