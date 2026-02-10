namespace OmegaSudoku.Core;

public readonly struct BoardChange(int row, int col, int oldMask, bool bitSet)
{
    public int Row { get; } = row;
    public int Col { get; } = col;
    public int OldMask { get; } = oldMask;
    public bool BitSet { get; } = bitSet;
}