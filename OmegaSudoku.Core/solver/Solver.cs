using System.Diagnostics;

namespace OmegaSudoku.Core;

public static class Solver
{
    public static readonly int[,] BoxLookup = new int[Board.Size, Board.Size];
    private const int AllMask = (1 << Board.Size) - 1;

    static Solver()
    {
        for (int r = 0; r < Board.Size; r++)
            for (int c = 0; c < Board.Size; c++)
                BoxLookup[r, c] = BoxIndex(r, c);
    }
    
    public static (bool, long) TimedSolve(int[,] board)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        bool solved = Solve(board);
        stopwatch.Stop();

        return (solved, stopwatch.ElapsedMilliseconds);
    }
    
    public static bool Solve(int[,] board)
    {
        int[] rowUsed = new int[Board.Size];
        int[] colUsed = new int[Board.Size];
        int[] boxUsed = new int[Board.Size];

        // Initialize the bitmasks for row, column, and box usage
        for (int i = 0; i < Board.Size; i++)
        {
            for (int j = 0; j < Board.Size; j++)
            {
                int cell = board[i, j];
                if (cell == 0) continue;
                
                int bit = 1 << (cell - 1);
                rowUsed[i] |= bit;
                colUsed[j] |= bit;
                boxUsed[BoxLookup[i, j]] |= bit;
            }
        }

        // Check every possibility and store the empty cells
        int[,] possibilities = new int[Board.Size, Board.Size];
        List<(int r, int c)> emptyCells = [];
        for (int r = 0; r < Board.Size; r++)
        {
            for (int c = 0; c < Board.Size; c++)
            {
                int cell = board[r, c];
                if (cell != 0) continue;

                int used = rowUsed[r] | colUsed[c] | boxUsed[BoxLookup[r, c]];
                possibilities[r, c] = AllMask & ~used;
                
                emptyCells.Add((r, c));
            }
        }

        return Backtracker.SolveRecursive(board, rowUsed, colUsed, boxUsed, possibilities, emptyCells, new Stack<(int r, int c, int oldMask, bool bitSet)>(200));
    }

    /// <summary>
    /// Calculates the box index for a given row and column.
    /// </summary>
    private static int BoxIndex(int row, int col)
    {
        return (row / Board.BoxSize) * Board.BoxSize + (col / Board.BoxSize);
    }
}