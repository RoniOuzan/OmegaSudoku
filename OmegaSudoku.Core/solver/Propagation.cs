using System.Numerics;

namespace OmegaSudoku.Core;

/// <summary>
/// Handles constraint propagation after a number is placed.
/// 
/// Uses forward-checking with cascading assignments:
/// - Removes invalid candidates from peers.
/// - Auto-assigns cells reduced to a single possibility.
/// - Detects contradictions early.
/// </summary>
public static class Propagation
{
    /// <summary>
    /// Propagates constraints starting from the specified cell.
    /// 
    /// Uses a queue-based approach to repeatedly eliminate invalid
    /// candidates from neighbors and trigger further assignments.
    /// 
    /// Returns <c>false</c> if a contradiction is detected.
    /// </summary>
    /// <returns>
    /// <c>true</c> if propagation succeeds without contradictions; 
    /// <c>false</c> if a contradiction is detected (a cell has no valid possibilities).
    /// </returns>
    public static bool UpdateNeighbors(SolverState state, int row, int col) {
        var queue = new Queue<Cell>();
        queue.Enqueue(new Cell(row, col));

        while (queue.Count > 0)
        {
            Cell cell = queue.Dequeue();
            int currentR = cell.Row;
            int currentC = cell.Col;
            
            int number = state.Board[currentR, currentC];
            int bit = 1 << (number - 1);

            for (int i = 0; i < state.Size; i++)
            {
                // Row
                if (i != currentC && 
                    !ProcessNeighbor(currentR, i, bit, state, queue)) 
                    return false;
                
                // Col
                if (i != currentR && 
                    !ProcessNeighbor(i, currentC, bit, state, queue)) 
                    return false;
            }

            // Box
            int boxR = (currentR / state.BoxSize) * state.BoxSize;
            int boxC = (currentC / state.BoxSize) * state.BoxSize;
            for (int i = boxR; i < boxR + state.BoxSize; i++)
            {
                if (i == currentR) continue;
                for (int j = boxC; j < boxC + state.BoxSize; j++)
                {
                    if (j == currentC) continue;
                    if (!ProcessNeighbor(i, j, bit, state, queue)) 
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Removes a candidate value from a neighboring cell.
    /// 
    /// If the cell is reduced to a single possibility, it is
    /// automatically assigned and added to the propagation queue.
    /// 
    /// Returns <c>false</c> if the cell becomes invalid.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the neighbor remains valid; 
    /// <c>false</c> if a contradiction is found.
    /// </returns>
    private static bool ProcessNeighbor(
        int r, 
        int c, 
        int bit, 
        SolverState state,
        Queue<Cell> queue
    ) {
        var board = state.Board;
        var rowUsed = state.RowUsed;
        var colUsed = state.ColUsed;
        var boxUsed = state.BoxUsed;
        var possibilities = state.Possibilities;
        var changes = state.Changes;
        
        // Ignore already filled cells
        if (board[r, c] != 0) return true;

        // If this bit is not present, nothing to remove
        if ((possibilities[r, c] & bit) == 0) return true;
        
        // Save previous state before modifying
        changes.Push(new BoardChange(r, c, possibilities[r, c], false));
        possibilities[r, c] &= ~bit;
        
        int count = BitOperations.PopCount((uint)possibilities[r, c]);
        // No possibilities left -> contradiction
        if (count == 0) return false;

        // If only one possibility remains, auto-assign
        if (count == 1)
        {
            int nextNum = BitOperations.TrailingZeroCount(possibilities[r, c]) + 1;
            int nextBit = 1 << (nextNum - 1);
            int boxIndex = state.BoxLookup[r, c];
            
            // Validate assignment against constraints
            if ((rowUsed[r] & nextBit) != 0 || (colUsed[c] & nextBit) != 0 ||
                (boxUsed[boxIndex] & nextBit) != 0)
                return false;

            // Mark this change as a committed placement
            var last = changes.Pop();
            changes.Push(new BoardChange(last.Row, last.Col, last.OldMask, true));

            board[r, c] = nextNum;
            rowUsed[r] |= nextBit;
            colUsed[c] |= nextBit;
            boxUsed[boxIndex] |= nextBit;
                
            possibilities[r, c] = 0;
            queue.Enqueue(new Cell(r, c));
        }
        
        return true;
    }
}