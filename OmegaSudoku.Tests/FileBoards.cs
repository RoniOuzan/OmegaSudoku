using OmegaSudoku.Core;

namespace OmegaSudoku.Tests;

public class FileBoards
{
    [Fact]
    public void BoardsFromFile()
    {
        var parent = Directory.GetParent(Directory.GetCurrentDirectory());
        string? projectDir = parent?.Parent?.Parent?.FullName;
        Assert.True(projectDir != null);
        string path = Path.Combine(projectDir, "boards.txt");;
        string[] lines = File.ReadAllLines(path);
        
        for (int i = 0; i < lines.Length; i++)
        {
            // Arrange
            int[,] board = Board.FromString(lines[i]);
            int[,] original = (int[,])board.Clone();
        
            // Act
            var (result, milliseconds) = Solver.TimedSolve(board);
            
            // Assert
            string flatBoard = Board.FlatString(original);
            Assert.True(result, $"Not solved: {flatBoard}");
            Assert.True(Board.IsValidSudoku(board), $"Finished but not valid: {flatBoard}");
            Assert.True(milliseconds < 1000, $"Took {milliseconds}ms for: {flatBoard}");
        }
    }
}