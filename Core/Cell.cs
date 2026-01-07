using System.Numerics;

namespace OmegaSudoku.Core;

public class Cell
{
    private int _number;
    private ulong _possibilities;
    private int _boardSize;

    public int Number
    {
        get => _number;
        set => _number = value;
    }
 
    public ulong Possibilities => this._possibilities;
    public int PossibilityCount => BitOperations.PopCount(this._possibilities);

    public Cell(int number, int boardSize)
    {
        this._number = number;
        this._boardSize = boardSize;

        if (number != 0)
            this._possibilities = 0;
        else
            this._possibilities = (1UL << boardSize) - 1;
    }
    
    public bool IsSolved => _number != 0;
    public bool IsEmpty => _number == 0;

    public void AddPossibility(int num) => this._possibilities |= 1UL << (num - 1);

    public void ClearPossibilities() {
        this._possibilities = 0;
    }

    public bool HasNoPossibilities() => this._possibilities == 0;
}