namespace OmegaSudoku.Core;

public readonly struct Masks(int size)
{
    public readonly int[] Row = new int[size];
    public readonly int[] Col = new int[size];
    public readonly int[] Box = new int[size];
}
