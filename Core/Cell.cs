namespace OmegaSudoku.Core;

public class Cell
{
    public int Number { get; set; }
    private List<int> Possibilities { get; }
    
    public Cell(int number)
    {
        this.Number = number;
        this.Possibilities = new List<int>();
    }

    public void AddPossibility(int num)
    {
        this.Possibilities.Add(num);
    }

    public bool IsEmpty => this.Number == 0;

    public override string ToString()
    {
        return this.Number.ToString();
    }
}