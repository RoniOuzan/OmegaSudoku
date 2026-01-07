namespace OmegaSudoku.Core;

public static class Solver
{
    public static bool Solve(Board board)
    {
        UpdatePossibilities(board);
        
        var (found, row, col) = FindBestEmptyCell(board);
        if (!found) return true; // Solved
        
        Cell cell = board.GetCell(row, col);

        for (var i = 0; i < cell.Possibilities.Count; i++)
        {
            int pos = cell.Possibilities[i];
            
            cell.Number = pos;
            if (Solve(board))
                return true;
            cell.Number = 0;
        }

        return false;
    }

    private static void UpdatePossibilities(Board board)
    {
        for (int i = 0; i < board.Size; i++)
        {
            for (int j = 0; j < board.Size; j++)
            {
                Cell cell = board.GetCell(i, j);
                if (cell.IsSolved)
                    continue;
                
                cell.ClearPossibilities();
                int a = 0;
                for (int num = 1; num <= board.Size; num++)
                {
                    if (board.IsSafe(i, j, num))
                    {
                        cell.AddPossibility(num);
                        a++;
                    }
                }

                if (cell.Possibilities.Count == 0 && !cell.IsSolved)
                    Console.WriteLine($"Cell ({i},{j}) has NO possibilities!");
            }
        }
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
                
                if (cell.Possibilities.Count < minPossibilities)
                {
                    minPossibilities = cell.Possibilities.Count;
                    foundRow = i;
                    foundCol = j;
                }
            }
        }

        return (foundRow != -1, foundRow, foundCol);
    }
}