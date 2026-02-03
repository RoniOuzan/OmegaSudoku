using OmegaSudoku.Core;

namespace OmegaSudoku.App;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello! i am your Sudoku Solver for today :)");
        Console.WriteLine("Give your sudoku board in 1 line (81 numbers in a row) and use 0 for empty cells.");

        while (true)
        {
            Console.WriteLine("\nEnter board:");
            
            string input = Console.ReadLine() ?? "";
            if (!IsInputValid(input)) continue;
            int[,] board = Board.FromString(input);
            
            var (solved, milliseconds) = Solver.TimedSolve(board);

            PrintResults(board, solved, milliseconds);
        }

        /*
         * 516849732307605000809700065135060907472591006968370050253186074684207500791050608
         * 000000000000003085001020000000507000004000100090000000500000073002010000000040009
         * 306508400520000000087000031003010080900863005050090600130000250000000074005206300
         * 000006000059000008200008000045000000003000000006003054000325006000000000000000000
         * 000870600200000000000100000060054000000000021400000000070000050000200300500001000
         */
    }

    static bool IsInputValid(string input)
    {
        input = string.Concat(input.Where(c => !char.IsWhiteSpace(c)));

        if (input.Length != Board.CellsCount)
        {
            Console.WriteLine($"Your input is {input.Length} characters long, it needs to be {Board.CellsCount}! ({Board.Size}x{Board.Size})");
            return false;
        }

        if (input.Any(c => c - '0' is < 0 or > Board.Size))
        {
            Console.WriteLine("Invalid character was found! Only digits are allowed.");
            return false;
        }

        return true;
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