namespace OmegaSudoku.Core;

public class Solver
{
    public static bool Solve(Board board, int row = 0, int column = 0)
    {
        // Next column
        if (column == board.Size)
        {
            column = 0;
            row++;
        }

        if (row == board.Size)
            return true;

        // Continue to the next
        if (!board.GetCell(row, column).IsEmpty)
            return Solve(board, row, column + 1);

        // Try every number
        for (int i = 1; i <= board.Size; i++)
        {
            if (!board.IsSafe(row, column, i)) 
                continue;
            
            board.GetCell(row, column).Number = i;
            if (Solve(board, row, column + 1))
                return true;
            board.GetCell(row, column).Number = 0;
        }

        return false;
    }
}