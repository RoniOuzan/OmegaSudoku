namespace OmegaSudoku.Core;

public static class Solver
{
    public static bool Solve(Board board)
    {
        int size = board.Size;

        List<int>[] rowUsed = new List<int>[size];
        List<int>[] colUsed = new List<int>[size];
        List<int>[] boxUsed = new List<int>[size];

        for (int i = 0; i < size; i++)
        {
            rowUsed[i] = new List<int>();
            colUsed[i] = new List<int>();
            boxUsed[i] = new List<int>();
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Cell cell = board.GetCell(i, j);
                if (cell.IsEmpty) continue;

                rowUsed[i].Add(cell.Number);
                colUsed[j].Add(cell.Number);
                boxUsed[BoxIndex(i, j, board.BoxSize)].Add(cell.Number);
            }
        }

        return SolveRecursive(board, rowUsed, colUsed, boxUsed);
    }

    private static bool SolveRecursive(Board board, List<int>[] rowUsed, List<int>[] colUsed, List<int>[] boxUsed)
    {
        var (possibilities, row, col) = FindBestEmptyCell(board, rowUsed, colUsed, boxUsed);
        if (row == -1) return true; // solved because no empty cells left
        if (possibilities.Count == 0) return false; // dead end because no possibilities

        foreach (int num in possibilities)
        {
            int boxIndex = BoxIndex(row, col, board.BoxSize);
            
            board.GetCell(row, col).Number = num;
            rowUsed[row].Add(num);
            colUsed[col].Add(num);
            boxUsed[boxIndex].Add(num);

            if (SolveRecursive(board, rowUsed, colUsed, boxUsed))
                return true;
            
            board.GetCell(row, col).Number = 0;
            rowUsed[row].Remove(num);
            colUsed[col].Remove(num);
            boxUsed[boxIndex].Remove(num);
        }

        return false;
    }

    private static (List<int>, int, int) FindBestEmptyCell(Board board, List<int>[] rowUsed, List<int>[] colUsed, List<int>[] boxUsed)
    {
        List<int> minPossibilities = new List<int>();
        int foundRow = -1;
        int foundCol = -1;

        for (int i = 0; i < board.Size; i++)
        {
            for (int j = 0; j < board.Size; j++)
            {
                var cell = board.GetCell(i, j);
                if (cell.IsSolved)
                    continue;

                List<int> possibilities = new List<int>();
                for (int num = 1; num <= board.Size; num++)
                {
                    if (!rowUsed[i].Contains(num) && 
                            !colUsed[j].Contains(num) &&
                            !boxUsed[BoxIndex(i, j, board.BoxSize)].Contains(num))
                        possibilities.Add(num);
                }

                // 1 because there won't be fewer possibilities, 0 because its dead end
                if (possibilities.Count == 1 || possibilities.Count == 0)
                    return (possibilities, i, j);
                
                if (foundRow == -1 || possibilities.Count < minPossibilities.Count)
                {
                    minPossibilities = possibilities;
                    foundRow = i;
                    foundCol = j;
                }
            }
        }

        return (minPossibilities, foundRow, foundCol);
    }
    
    private static int BoxIndex(int row, int column, int boxSize)
    {
        return (row / boxSize) * boxSize + (column / boxSize);
    }
}