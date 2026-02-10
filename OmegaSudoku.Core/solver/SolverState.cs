namespace OmegaSudoku.Core;

public readonly struct SolverState(
    int[,] board,
    int[] rowUsed,
    int[] colUsed,
    int[] boxUsed,
    int[,] possibilities,
    List<Cell> emptyCells,
    Stack<BoardChange> changes)
{
    public readonly int[,] Board = board;
    public readonly int[] RowUsed = rowUsed;
    public readonly int[] ColUsed = colUsed;
    public readonly int[] BoxUsed = boxUsed;
    public readonly int[,] Possibilities = possibilities;
    public readonly List<Cell> EmptyCells = emptyCells;
    public readonly Stack<BoardChange> Changes = changes;
}