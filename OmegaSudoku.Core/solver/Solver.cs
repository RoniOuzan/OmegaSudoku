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
    public static (bool finished, long ms) TimedSolve(int[,] board)
    {
        var stopwatch = Stopwatch.StartNew();
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
        if (!IsValidBoard(board))
            return false;
        
        SolverState? state = InitializeState(board);
        if (state == null) // Board is unsolvable
            return false;
        
        return Backtracker.SolveRecursive(state);
    }

    /// <summary>
    /// Validates the structural shape of the board.
    /// Ensures it is square and its size forms valid sub-boxes.
    /// </summary>
    /// <returns><c>true</c> if the board shape is valid; otherwise, <c>false</c>.</returns>
    private static bool IsValidBoard(int[,] board)
    {
        int size = board.GetLength(0);
        if (size != board.GetLength(1))
            return false;

        int boxSize = (int)Math.Sqrt(size);
        if (boxSize * boxSize != size)
            return false;

        return true;
    }

    /// <summary>
    /// Builds the <see cref="SolverState"/> used by the backtracking engine.
    /// Initializes masks, candidate values, and empty cell tracking.
    /// Returns <c>null</c> if the board is invalid.
    /// </summary>
    /// <returns>The initialized solver state, or <c>null</c> if invalid.</returns>
    private static SolverState? InitializeState(int[,] board)
    {
        int size = board.GetLength(0);
        var boxSize = (int)Math.Sqrt(size);
        int[,] boxLookup = CreateBoxLookup(size, boxSize);

        if (!TryInitializeMasks(board, size, boxLookup, out Masks masks))
            return null;

        int[,] availableNumbers = CreateInitialPossibilities(board, size, boxLookup, masks);
        List<Cell> emptyCells = CollectEmptyCells(board, size);
        
        return new SolverState(
            board,
            size,
            boxSize,
            boxLookup,
            masks.Row,
            masks.Col,
            masks.Box,
            availableNumbers,
            emptyCells,
            new Stack<BoardChange>()
        );
    }

    /// <summary>
    /// Builds row, column, and box constraint masks from the board.
    /// Validates value ranges and detects duplicate conflicts.
    /// </summary>
    /// <returns><c>true</c> if valid; otherwise, <c>false</c>.</returns>
    private static bool TryInitializeMasks(int[,] board, int size, int[,] boxLookup, out Masks masks)
    {
        masks = new Masks(size);

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                int cell = board[r, c];
                if (cell == 0) continue;
                
                if (cell < 1 || cell > size)
                    return false;

                int bit = 1 << (cell - 1);
                int box = boxLookup[r, c];

                if ((masks.Row[r] & bit) != 0 ||
                    (masks.Col[c] & bit) != 0 ||
                    (masks.Box[box] & bit) != 0)
                    return false;

                masks.Row[r] |= bit;
                masks.Col[c] |= bit;
                masks.Box[box] |= bit;
            }
        }

        return true;
    }

    /// <summary>
    /// Creates a lookup table mapping each cell to its box index.
    /// </summary>
    /// <returns>A 2D array of box indices.</returns>
    private static int[,] CreateBoxLookup(int size, int boxSize)
    {
        var boxLookup = new int[size, size];
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                boxLookup[r, c] = Board.GetBoxIndex(r, c, boxSize);
        
        return boxLookup;
    }

    /// <summary>
    /// Computes initial candidate masks for all empty cells.
    /// </summary>
    /// <returns>A 2D array of candidate bitmasks.</returns>
    private static int[,] CreateInitialPossibilities(int[,] board, int size, int[,] boxLookup, Masks masks)
    {
        var availableNumbers = new int[size, size];
        int allMask = (1 << size) - 1;
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                int cell = board[r, c];
                if (cell != 0) continue;

                int used = masks.Row[r] | masks.Col[c] | masks.Box[boxLookup[r, c]];
                availableNumbers[r, c] = allMask & ~used;
            }
        }

        return availableNumbers;
    }

    /// <summary>
    /// Collects coordinates of all empty cells in the board.
    /// </summary>
    /// <returns>A list of empty cell positions.</returns>
    private static List<Cell> CollectEmptyCells(int[,] board, int size)
    {
        List<Cell> emptyCells = [];
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                if (board[r, c] == 0)
                    emptyCells.Add(new Cell(r, c));
        
        return emptyCells;
    }
}