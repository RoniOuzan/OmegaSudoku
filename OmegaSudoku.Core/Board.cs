namespace OmegaSudoku.Core;

public static class Board
{

    /// <summary>
    /// Prints the board to the console in a formatted grid layout.
    /// </summary>
    /// <param name="board">The Sudoku board.</param>
    public static void Print(int[,] board)
    {
        int size = board.GetLength(0);
        int boxSize = (int)Math.Sqrt(size);
        for (int r = 0; r < size; r++)
        {
            if (r > 0 && r % boxSize == 0)
                Console.WriteLine("---------+---------+---------");

            for (int c = 0; c < size; c++)
            {
                if (c > 0 && c % boxSize == 0)
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
        int size = board.GetLength(0);
        var text = string.Empty;
        for (int i = 0; i < size; i++) 
            for (int j = 0; j < size; j++)
                text += board[i, j];

        return text;
    }

    /// <summary>
    /// Creates a Sudoku board from a string containing 81 digits.
    /// Whitespace is ignored.
    /// </summary>
    /// <param name="input">String containing the board values.</param>
    /// <param name="size">Size of the sudoku board</param>
    /// <returns>A 9x9 Sudoku board.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the input does not contain exactly 81 digits or invalid character.
    /// </exception>
    public static int[,] FromString(string input, int size)
    {
        int cellsCount = size * size;
        input = string.Concat(input.Where(c => !char.IsWhiteSpace(c)));

        if (input.Length != cellsCount)
            throw new ArgumentException($"Your input has {input.Length} digits, must contain exactly {cellsCount} digits.");

        var board = new int[size, size];
        for (int i = 0; i < cellsCount; i++)
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

            board[i / size, i % size] = c - '0';
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
        int size = board.GetLength(0);
        int boxSize = (int)Math.Sqrt(size);
        
        // Check rows and columns
        for (int i = 0; i < size; i++)
        {
            var rowCheck = new bool[size];
            var colCheck = new bool[size];

            for (int j = 0; j < size; j++)
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
        for (int boxRow = 0; boxRow < boxSize; boxRow++)
        {
            for (int boxCol = 0; boxCol < boxSize; boxCol++)
            {
                var boxCheck = new bool[size];

                for (int r = boxRow * boxSize; r < boxRow * boxSize + boxSize; r++)
                {
                    for (int c = boxCol * boxSize; c < boxCol * boxSize + boxSize; c++)
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


    /// <summary>
    /// Calculates the box index for a given row and column.
    /// Box index is determined by which 3x3 (or general BoxSize x BoxSize) box contains the cell.
    /// </summary>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="col">The column index of the cell.</param>
    /// <param name="boxSize">The size of a single box in the sudoku board</param>
    /// <returns>The index of the box that contains the cell. Boxes are indexed left-to-right, top-to-bottom, starting at 0.</returns>
    public static int GetBoxIndex(int row, int col, int boxSize)
    {
        return (row / boxSize) * boxSize + (col / boxSize);
    }
}
