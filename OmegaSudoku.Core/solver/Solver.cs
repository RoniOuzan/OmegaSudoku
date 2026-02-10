using System.Diagnostics;

namespace OmegaSudoku.Core;

/// <summary>
/// Entry point for solving Sudoku boards.
/// 
/// This class is responsible for:
/// 1. Preparing the internal solver state from an input board.
/// 2. Delegating the actual search to the backtracking engine.
/// 
/// The recursive search logic is intentionally separated into <see cref="Backtracker"/>
/// to keep initialization and search concerns isolated.
/// </summary>
public static class Solver
{
    /// Lookup table to convert a (row, column) pair to the corresponding box index on the Sudoku board.
    public static readonly int[,] BoxLookup = new int[Board.Size, Board.Size];
    /// Bitmask with all bits set for the size of the board (used for checking available numbers in cells).
    private const int AllMask = (1 << Board.Size) - 1;

    static Solver()
    {
        // Initialize Box Lookup
        for (int r = 0; r < Board.Size; r++)
            for (int c = 0; c < Board.Size; c++)
                BoxLookup[r, c] = Board.GetBoxIndex(r, c);
    }
    
    /// <summary>
    /// Attempts to solve the given Sudoku board efficiently using bitmask, backtracking, MRV, propagation etc.
    /// Updates the given board with the solved board and returns whether it was solved along with the time taken.
    /// </summary>
    /// <param name="board">A 2D  array representing the Sudoku board. Empty cells are represented by 0</param>
    /// <returns>
    /// A tuple containing:
    /// - <c>solved</c>: whether a valid solution was found.
    /// - <c>ms</c>: elapsed time in milliseconds.
    /// </returns>
    public static (bool solved, long ms) TimedSolve(int[,] board)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        bool solved = Solve(board);
        stopwatch.Stop();

        return (solved, stopwatch.ElapsedMilliseconds);
    }
    
    /// <summary>
    /// Attempts to solve the given Sudoku board using bitmask, backtracking, MRV, propagation etc.
    /// </summary>
    /// <param name="board">A 2D  array representing the Sudoku board. Empty cells are represented by 0.</param>
    /// <returns><c>true</c> if the board was successfully solved; otherwise, <c>false</c>.</returns>
    public static bool Solve(int[,] board)
    {
        SolverState state = InitializeState(board);
        return Backtracker.SolveRecursive(state);
    }

    /// <summary>
    /// Builds the internal <see cref="SolverState"/> required by the backtracking engine.
    /// 
    /// This method:
    /// - Initializes row/column/box usage bitmasks.
    /// - Computes initial possibility masks for all empty cells.
    /// - Collects coordinates of empty cells.
    /// </summary>
    private static SolverState InitializeState(int[,] board)
    {
        var rowUsed = new int[Board.Size];
        var colUsed = new int[Board.Size];
        var boxUsed = new int[Board.Size];

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
        List<Cell> emptyCells = [];
        for (int r = 0; r < Board.Size; r++)
        {
            for (int c = 0; c < Board.Size; c++)
            {
                int cell = board[r, c];
                if (cell != 0) continue;

                int used = rowUsed[r] | colUsed[c] | boxUsed[BoxLookup[r, c]];
                possibilities[r, c] = AllMask & ~used;
                
                emptyCells.Add(new Cell(r, c));
            }
        }
        
        return new SolverState(
            board,
            rowUsed,
            colUsed,
            boxUsed,
            possibilities,
            emptyCells,
            new Stack<BoardChange>()
        );
    }
}