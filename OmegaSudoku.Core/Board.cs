namespace OmegaSudoku.Core;

public static class Board
{
    /// <summary>
    /// Size of the Sudoku board (number of rows and columns).
    /// </summary>
    public const int Size = 9;

    /// <summary>
    /// Total number of cells on the board.
    /// </summary>
    public const int CellsCount = Size * Size;

    /// <summary>
    /// Size of one sub-box (e.g., 3 for a 9x9 Sudoku).
    /// </summary>
    public static readonly int BoxSize = (int)Math.Sqrt(Size);

    /// <summary>
    /// Prints the board to the console in a formatted grid layout.
    /// </summary>
    /// <param name="board">The Sudoku board.</param>
    public static void Print(int[,] board)
    {
        for (int r = 0; r < Size; r++)
        {
            if (r > 0 && r % BoxSize == 0)
                Console.WriteLine("---------+---------+---------");

            for (int c = 0; c < Size; c++)
            {
                if (c > 0 && c % BoxSize == 0)
                    Console.Write("|");

                Console.Write($" {board[r, c]} ");
            }

            Console.WriteLine();
        }
    }

    /// <summary>
    /// Converts the board into a single flat string of digits (row-major order).
    /// </summary>
    /// <param name="board">The Sudoku board.</param>
    /// <returns>A string representation of all 81 cells.</returns>
    public static string FlatString(int[,] board)
    {
        string text = string.Empty;
        for (int i = 0; i < Size; i++) 
            for (int j = 0; j < Size; j++)
                text += board[i, j];

        return text;
    }

    /// <summary>
    /// Creates a Sudoku board from a string containing 81 digits.
    /// Whitespace is ignored.
    /// </summary>
    /// <param name="input">String containing the board values.</param>
    /// <returns>A 9x9 Sudoku board.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the input does not contain exactly 81 digits or invalid character.
    /// </exception>
    public static int[,] FromString(string input)
    {
        input = string.Concat(input.Where(c => !char.IsWhiteSpace(c)));

        if (input.Length != CellsCount)
            throw new ArgumentException($"Your input has {input.Length} digits, must contain exactly {CellsCount} digits.");

        int[,] board = new int[Size, Size];
        for (int i = 0; i < CellsCount; i++)
        {
            char c = input[i];
            if (!char.IsDigit(c))
            {
                string pointerLine = new string(' ', i) + "^";
                throw new ArgumentException(
                    $"Invalid character found at index {i}:\n" +
                    $"{input}\n" +
                    $"{pointerLine}");
            }

            board[i / Size, i % Size] = c - '0';
        }

        return board;
    }

    /// <summary>
    /// Determines whether a fully filled Sudoku board is valid.
    /// </summary>
    /// <param name="board">The Sudoku board.</param>
    /// <returns>
    /// <c>true</c> if the board satisfies all Sudoku constraints; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValidSudoku(int[,] board)
    {
        // Check rows and columns
        for (int i = 0; i < Size; i++)
        {
            bool[] rowCheck = new bool[Size];
            bool[] colCheck = new bool[Size];

            for (int j = 0; j < Size; j++)
            {
                int row = board[i, j];
                int col = board[j, i];

                if (rowCheck[row - 1])
                    return false;

                if (colCheck[col - 1])
                    return false;

                rowCheck[row - 1] = true;
                colCheck[col - 1] = true;
            }
        }

        // Check boxes
        for (int boxRow = 0; boxRow < BoxSize; boxRow++)
        {
            for (int boxCol = 0; boxCol < BoxSize; boxCol++)
            {
                bool[] boxCheck = new bool[Size];

                for (int r = boxRow * BoxSize; r < boxRow * BoxSize + BoxSize; r++)
                {
                    for (int c = boxCol * BoxSize; c < boxCol * BoxSize + BoxSize; c++)
                    {
                        int cell = board[r, c];

                        if (boxCheck[cell - 1])
                            return false;

                        boxCheck[cell - 1] = true;
                    }
                }
            }
        }

        return true;
    }
}
