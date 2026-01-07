namespace OmegaSudoku.Core;

public class Cell
{
    private int _number;

    public int Number
    {
        get => _number;
        set => _number = value;
    }
 
    public Cell(int number)
    {
        this._number = number;
    }
    
    public bool IsSolved => this._number != 0;
    public bool IsEmpty => this._number == 0;
}