using OmegaSudoku.Core;

namespace OmegaSudoku.Tests;

public class FileBoardsTests
{
    [Fact]
    public void BoardsFromFile()
    {
        DirectoryInfo? parent = Directory.GetParent(Directory.GetCurrentDirectory());
        string? projectDir = parent?.Parent?.Parent?.FullName;
        Assert.True(projectDir != null);
        string path = Path.Combine(projectDir, "boards.txt");;
        string[] lines = File.ReadAllLines(path);
        
        foreach (string line in lines)
        {
            // Arrange
            int[,] board = Board.FromString(line, 9);
            var original = (int[,])board.Clone();
        
            // Act
            (bool solved, long milliseconds) = Solver.TimedSolve(board);
            
            // Assert
            string flatBoard = Board.FlatString(original);
            Assert.True(solved, $"Not solved: {flatBoard}");
            Assert.True(Board.IsValidSudoku(board), $"Finished but not valid: {flatBoard}");
            Assert.True(milliseconds < 1000, $"Took {milliseconds}ms for: {flatBoard}");
        }
    }
}