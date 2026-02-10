namespace OmegaSudoku.Core;

public readonly struct BoardChange(int row, int col, int oldMask, bool bitSet)
{
    public readonly int Row = row;
    public readonly int Col = col;
    public readonly int OldMask = oldMask;
    public readonly bool BitSet = bitSet;
}