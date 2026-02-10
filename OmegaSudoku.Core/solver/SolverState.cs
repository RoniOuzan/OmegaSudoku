namespace OmegaSudoku.Core;

/// <summary>
/// Holds the current state of the board for the backtracking solver.
/// </summary>
public class SolverState(
    int[,] board,
    int size,
    int[,] boxLookup,
    int[] rowUsed,
    int[] colUsed,
    int[] boxUsed,
    int[,] availableNumbers,
    List<Cell> emptyCells,
    Stack<BoardChange> changes)
{
    /// <summary>Current board numbers, 0 for empty.</summary>
    public readonly int[,] Board = board;
    /// <summary>Size of the board.</summary>
    public readonly int Size = size;
    /// <summary>Size of a single box in the board.</summary>
    public readonly int BoxSize = (int)Math.Sqrt(size);
    /// <summary>Lookup table to convert a (row, column) pair to the corresponding box index on the board.</summary>
    public readonly int[,] BoxLookup = boxLookup;
    /// <summary>Which numbers are used in each row.</summary>
    public readonly int[] RowUsed = rowUsed;
    /// <summary>Which numbers are used in each column.</summary>
    public readonly int[] ColUsed = colUsed;
    /// <summary>Which numbers are used in each box.</summary>
    public readonly int[] BoxUsed = boxUsed;
    /// <summary>Bitmask of available numbers per cell.</summary>
    public readonly int[,] AvailableNumbers = availableNumbers;
    /// <summary>Coordinates of empty cells.</summary>
    public readonly List<Cell> EmptyCells = emptyCells;
    /// <summary>Changes stack used for backtracking.</summary>
    public readonly Stack<BoardChange> Changes = changes;
}