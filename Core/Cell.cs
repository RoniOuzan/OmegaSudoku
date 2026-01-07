namespace OmegaSudoku.Core;

public class Cell
{
    private int _number;
    private List<int> _possibilities;

    public int Number
    {
        get => _number;
        set => _number = value;
    }

    public List<int> Possibilities => _possibilities;

    public Cell(int number)
    {
        _number = number;
        _possibilities = new List<int>();
    }
    
    public bool IsSolved => _number != 0;
    public bool IsEmpty => _number == 0;

    public void AddPossibility(int num) => _possibilities.Add(num);

    public void ClearPossibilities() => _possibilities.Clear();
}