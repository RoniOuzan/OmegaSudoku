using System.Diagnostics;
using OmegaSudoku.Core;

namespace OmegaSudoku;

class Program
{
    static void Main(string[] args)
    {
        int[,] board = 
        {
            {3, 0, 6, 5, 0, 8, 4, 0, 0}, 
            {5, 2, 0, 0, 0, 0, 0, 0, 0}, 
            {0, 8, 7, 0, 0, 0, 0, 3, 1},
            {0, 0, 3, 0, 1, 0, 0, 8, 0}, 
            {9, 0, 0, 8, 6, 3, 0, 0, 5}, 
            {0, 5, 0, 0, 9, 0, 6, 0, 0},
            {1, 3, 0, 0, 0, 0, 2, 5, 0}, 
            {0, 0, 0, 0, 0, 0, 0, 7, 4}, 
            {0, 0, 5, 2, 0, 6, 3, 0, 0}
        };
        
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        Solver.Solve(board);
        
        stopwatch.Stop();
        
        PrintBoard(board);
        Console.WriteLine($"\nSolved in {stopwatch.ElapsedMilliseconds} ms");
    }

    static void PrintBoard(int[,] board)
    {
        for (int i = 0; i < board.GetLength(0); i++) {
            if (i > 0 && i % 3 == 0)
            {
                Console.WriteLine("---------+---------+---------");
            }

            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (j > 0 && j % 3 == 0)
                {
                    Console.Write("|");
                }
                Console.Write(" " + board[i, j] + " ");
            }
            Console.WriteLine();
        }
    }
}