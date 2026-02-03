namespace OmegaSudoku.Core;

public class Board
{
    public const int Size = 9;
    public static readonly int CellsCount = Size * Size;
    public static readonly int BoxSize = (int)Math.Sqrt(Size);

    public static void Print(int[,] board)
    {
        for (int i = 0; i < Size; i++) 
        {
            if (i > 0 && i % BoxSize == 0)
                Console.WriteLine("---------+---------+---------");

            for (int j = 0; j < Size; j++)
            {
                if (j > 0 && j % BoxSize == 0)
                    Console.Write("|");
                Console.Write(" " + board[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    public static int[,] FromString(string input)
    {
        int[,] board = new int[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                board[i, j] = input[i * Size + j] - '0';
            }
        }

        return board;
    }
}