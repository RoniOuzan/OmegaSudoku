namespace OmegaSudoku.Core;

public readonly struct Cell(int row, int col)
{
    public readonly int Row = row;
    public readonly int Col = col;
}