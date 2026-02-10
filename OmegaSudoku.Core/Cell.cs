namespace OmegaSudoku.Core;

public readonly struct Cell(int row, int col)
{
    public int Row { get; } = row;
    public int Col { get; } = col;
}