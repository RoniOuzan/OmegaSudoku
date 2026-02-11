namespace OmegaSudoku.Core;

/// <summary>
/// Holds the current state of the board for the backtracking solver.
/// </summary>
public class SolverState {
    /// <summary>Current board numbers, 0 for empty.</summary>
    public readonly int[,] Board;
    /// <summary>Size of the board.</summary>
    public readonly int Size;
    /// <summary>Size of a single box in the board.</summary>
    public readonly int BoxSize;
    /// <summary>Lookup table to convert a (row, column) pair to the corresponding box index on the board.</summary>
    public readonly int[,] BoxLookup;
    /// <summary>Which numbers are used in each row.</summary>
    public readonly int[] RowUsed;
    /// <summary>Which numbers are used in each column.</summary>
    public readonly int[] ColUsed;
    /// <summary>Which numbers are used in each box.</summary>
    public readonly int[] BoxUsed;
    /// <summary>Bitmask of available numbers per cell.</summary>
    public readonly int[,] AvailableNumbers;
    /// <summary>Coordinates of empty cells.</summary>
    public readonly List<Cell> EmptyCells;
    /// <summary>Changes stack used for backtracking.</summary>
    public readonly Stack<BoardChange> Changes;

    internal SolverState(
        int[,] board,
        int size,
        int boxSize,
        int[,] boxLookup,
        int[] rowUsed,
        int[] colUsed,
        int[] boxUsed,
        int[,] availableNumbers,
        List<Cell> emptyCells,
        Stack<BoardChange> changes)
    {
        this.Board = board;
        this.Size = size;
        this.BoxSize = boxSize;
        this.BoxLookup = boxLookup;
        this.RowUsed = rowUsed;
        this.ColUsed = colUsed;
        this.BoxUsed = boxUsed;
        this.AvailableNumbers = availableNumbers;
        this.EmptyCells = emptyCells;
        this.Changes = changes;
    }
}