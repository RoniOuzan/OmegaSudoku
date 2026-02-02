using System;

namespace OmegaSudoku.Core;

public class Board
{
    public static void Print(int[,] board)
    {
        for (int i = 0; i < 9; i++) 
        {
            if (i > 0 && i % 3 == 0)
                Console.WriteLine("---------+---------+---------");

            for (int j = 0; j < 9; j++)
            {
                if (j > 0 && j % 3 == 0)
                    Console.Write("|");
                Console.Write(" " + board[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    public static int[,] FromString(string input)
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

        return board;
    }
}