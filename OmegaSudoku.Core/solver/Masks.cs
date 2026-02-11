namespace OmegaSudoku.Core;

public readonly struct Masks
{
    public readonly int[] Row;
    public readonly int[] Col;
    public readonly int[] Box;

    public Masks(int size)
    {
        this.Row = new int[size];
        this.Col = new int[size];
        this.Box = new int[size];
    }
}
