using OmegaSudoku.Core;

namespace OmegaSudoku.App;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello! i am your Sudoku Solver for today :)");
        Console.WriteLine("Give your sudoku board in 1 line (81 numbers in a row) and use 0 for empty cells.");

        while (true)
        {
            try
            {
                Console.WriteLine("\nEnter board:");
                
                string input = Console.ReadLine() ?? "";
                int[,] board = Board.FromString(input, 9);
            
                (bool solved, long milliseconds) = Solver.TimedSolve(board);

                PrintResults(board, solved, milliseconds);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    private static void PrintResults(int[,] board, bool solved, long milliseconds)
    {
        Console.WriteLine();
        if (solved)
        {
            Board.Print(board);
            Console.WriteLine(Board.IsValidSudoku(board)
                ? $"Solved in {milliseconds} ms!"
                : $"Didn't solved in {milliseconds} ms!");
        }
        else
        {
            Console.WriteLine($"This board is unsolvable! It took {milliseconds} ms to detect.");
        }
    }
}