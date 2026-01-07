namespace OmegaSudoku.Core;

public class Cell
{
    private int _number;
    private int[] _possibilities;
    private int _possibilitiesCount;

    public int Number
    {
        get => _number;
        set => _number = value;
    }
 
    public int[] Possibilities => this._possibilities;
    public int PossibilityCount => this._possibilitiesCount;

    public Cell(int number, int boardSize)
    {
        this._number = number;
        this._possibilities = new int[boardSize];
        this._possibilitiesCount = 0;
    }
    
    public bool IsSolved => _number != 0;
    public bool IsEmpty => _number == 0;

    public void AddPossibility(int num) => this._possibilities[this._possibilitiesCount++] = num;

    public void ClearPossibilities() {
        this._possibilitiesCount = 0;
        Array.Fill(this._possibilities, 0);
    }

    public bool HasNoPossibilities() => this._possibilitiesCount == 0;
}