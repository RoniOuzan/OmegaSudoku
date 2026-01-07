namespace OmegaSudoku.Core;

public class Board
{
    private readonly Cell[,] _cells;
    public int Size { get; }
    public int BoxSize { get; }

    public Board(int[,] cells)
    {
        this.Size = cells.GetLength(0);
        this.BoxSize = (int) Math.Sqrt(this.Size);
        
        this._cells = new Cell[this.Size, this.Size];
        for (int i = 0; i < this.Size; i++)
        {
            for (int j = 0; j < this.Size; j++)
            {
                this._cells[i, j] = new Cell(cells[i, j]);
            }
        }
    }
    
    public Cell GetCell(int row, int col)
    {
        return this._cells[row, col];
    }
    
    public void Print()
    {
        for (int i = 0; i < this.Size; i++) 
        {
            if (i > 0 && i % this.BoxSize == 0)
                Console.WriteLine("---------+---------+---------");

            for (int j = 0; j < this.Size; j++)
            {
                if (j > 0 && j % this.BoxSize == 0)
                    Console.Write("|");
                Console.Write(" " + this.GetCell(i, j).Number + " ");
            }
            Console.WriteLine();
        }
    }

    public override string ToString()
    {
        string result = "";
        for (int i = 0; i < this.Size; i++)
            for (int j = 0; j < this.Size; j++)
                result += $"{this.GetCell(i, j).Number}";

        return result;
    }
}