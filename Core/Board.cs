using System;

namespace OmegaSudoku.Core;

public class Board
{
    private readonly int[,] _cells;
    public int Size { get; }
    public int BoxSize { get; }

    public Board(int[,] cells)
    {
        this.Size = cells.GetLength(0);
        this.BoxSize = (int) Math.Sqrt(this.Size);
        
        this._cells = new int[this.Size, this.Size];
        for (int i = 0; i < this.Size; i++)
        {
            for (int j = 0; j < this.Size; j++)
            {
                this._cells[i, j] = cells[i, j];
            }
        }
    }
    
    public int GetCell(int row, int col)
    {
        return this._cells[row, col];
    }
    
    public void SetCell(int row, int col, int num)
    {
        this._cells[row, col] = num; 
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
                Console.Write(" " + this.GetCell(i, j) + " ");
            }
            Console.WriteLine();
        }
    }

    public override string ToString()
    {
        string result = "";
        for (int i = 0; i < this.Size; i++)
            for (int j = 0; j < this.Size; j++)
                result += $"{this.GetCell(i, j)}";

        return result;
    }

    public static Board FromString(string input)
    {
        int size = (int)Math.Sqrt(input.Length);
        int[,] board = new int[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                board[i, j] = input[i * size + j] - '0';
            }
        }

        return new Board(board);
    }
}