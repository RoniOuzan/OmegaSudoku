namespace OmegaSudoku.Core;

public static class Solver
{
    public static bool Solve(Board board)
    {
        if (!UpdatePossibilities(board))
            return false;
        
        var (found, row, col) = FindBestEmptyCell(board);
        if (!found) return true; // Solved
        
        Cell cell = board.GetCell(row, col);

        foreach (int num in cell.EnumeratePossibilities())
        {
            cell.Number = num;
            if (Solve(board)) return true;
            cell.Number = 0;
        }

        return false;
    }

    private static bool UpdatePossibilities(Board board)
    {
        for (int i = 0; i < board.Size; i++)
        {
            for (int j = 0; j < board.Size; j++)
            {
                Cell cell = board.GetCell(i, j);
                if (cell.IsSolved)
                    continue;
                
                cell.ClearPossibilities();
                for (int num = 1; num <= board.Size; num++)
                    if (board.IsSafe(i, j, num))
                        cell.AddPossibility(num);

                // if (cell.PossibilityCount == 1)
                // {
                //     cell.Number = cell.Possibilities[0];
                // }

                if (cell.HasNoPossibilities() && !cell.IsSolved)
                    return false;
            }
        }

        return true;
    }

    private static (bool, int, int) FindBestEmptyCell(Board board)
    {
        int minPossibilities = int.MaxValue;
        int foundRow = -1;
        int foundCol = -1;

        for (int i = 0; i < board.Size; i++)
        {
            for (int j = 0; j < board.Size; j++)
            {
                var cell = board.GetCell(i, j);
                if (cell.IsSolved)
                    continue;

                int count = cell.PossibilityCount;
                if (count < minPossibilities)
                {
                    minPossibilities = count;
                    foundRow = i;
                    foundCol = j;
                }
            }
        }

        return (foundRow != -1, foundRow, foundCol);
    }
}