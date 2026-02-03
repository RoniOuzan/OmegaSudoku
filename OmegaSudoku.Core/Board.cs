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

    public static string FlatString(int[,] board)
    {
        string text = string.Empty;
        
        for (int i = 0; i < Size; i++) 
        {
            for (int j = 0; j < Size; j++)
            {
                text += board[i, j];
            }
        }

        return text;
    }

    public static int[,] FromString(string input)
    {
        input = string.Concat(input.Where(c => !char.IsWhiteSpace(c)));
        
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
    
    public static bool IsValidSudoku(int[,] board)
    {
        // Row and Cols
        for (int i = 0; i < 9; i++)
        {
            bool[] rowCheck = new bool[9];
            bool[] colCheck = new bool[9];
            for (int j = 0; j < 9; j++)
            {
                int r = board[i,j];
                int c = board[j,i];
                if (r < 1 || r > 9 || rowCheck[r-1]) return false;
                if (c < 1 || c > 9 || colCheck[c-1]) return false;
                rowCheck[r-1] = true;
                colCheck[c-1] = true;
            }
        }

        // Boxes
        for (int boxRow = 0; boxRow < 3; boxRow++)
        {
            for (int boxCol = 0; boxCol < 3; boxCol++)
            {
                bool[] boxCheck = new bool[9];
                for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                {
                    int val = board[r,c];
                    if (val < 1 || val > 9 || boxCheck[val-1]) return false;
                    boxCheck[val-1] = true;
                }
            }
        }

        return true;
    }
}