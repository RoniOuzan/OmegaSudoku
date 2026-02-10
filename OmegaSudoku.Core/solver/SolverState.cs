namespace OmegaSudoku.Core;

public class SolverState(
    int[,] board,
    int size,
    int[,] boxLookup,
    int[] rowUsed,
    int[] colUsed,
    int[] boxUsed,
    int[,] possibilities,
    List<Cell> emptyCells,
    Stack<BoardChange> changes)
{
    public readonly int[,] Board = board;
    public readonly int Size = size;
    public readonly int BoxSize = (int)Math.Sqrt(size);
    public readonly int[,] BoxLookup = boxLookup;
    public readonly int[] RowUsed = rowUsed;
    public readonly int[] ColUsed = colUsed;
    public readonly int[] BoxUsed = boxUsed;
    public readonly int[,] Possibilities = possibilities;
    public readonly List<Cell> EmptyCells = emptyCells;
    public readonly Stack<BoardChange> Changes = changes;
}