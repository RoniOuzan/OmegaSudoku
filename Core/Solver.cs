namespace OmegaSudoku.Core;

public class Solver
{
    public static bool Solve(int[,] board, int column = 0, int row = 0)
    {
        if (row == board.GetLength(1))
        {
            row = 0;
            column++;
        }

        if (column == board.GetLength(0))
            return true;

        if (board[column, row] != 0)
            return Solve(board, column, row + 1);

        for (int i = 1; i <= board.GetLength(0); i++)
        {
            if (!isSafe(board, column, row, i)) 
                continue;
            
            board[column, row] = i;
            if (Solve(board, column, row + 1))
                return true;
            board[column, row] = 0;
        }

        return false;
    }

    public static bool isSafe(int[,] board, int column, int row, int num)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            if (board[i, row] == num)
                return false;
        }
        
        for (int i = 0; i < board.GetLength(0); i++)
        {
            if (board[column, i] == num)
                return false;
        }

        int startCol = column - (column % 3);
        int startRow = row - (row % 3);
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (board[i + startCol, j + startRow] == num)
                    return false;

        return true;
    }
}