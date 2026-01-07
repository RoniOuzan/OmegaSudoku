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

    public bool IsSafe(int row, int column, int num)
    {
        // Check column
        for (int i = 0; i < this.Size; i++)
        {
            if (this.GetCell(i, column).Number == num)
                return false;
        }
        
        // Check row
        for (int i = 0; i < this.Size; i++)
        {
            if (this.GetCell(row, i).Number == num)
                return false;
        }

        // Check box
        int startCol = row - (row % 3);
        int startRow = column - (column % 3);
        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
            if (this.GetCell(i + startCol, j + startRow).Number == num)
                return false;

        return true;
    }
    
    public void Print()
    {
        for (int i = 0; i < this.Size; i++) {
            if (i > 0 && i % 3 == 0)
            {
                Console.WriteLine("---------+---------+---------");
            }

            for (int j = 0; j < this.Size; j++)
            {
                if (j > 0 && j % 3 == 0)
                {
                    Console.Write("|");
                }
                Console.Write(" " + this.GetCell(i, j) + " ");
            }
            Console.WriteLine();
        }
    }

    public override string ToString()
    {
        string result = "";
        for (int i = 0; i < this.Size; i++)
        {
            for (int j = 0; j < this.Size; j++)
            {
                result += $"{this.GetCell(i, j)}";
            }
        }

        return result;
    }
}