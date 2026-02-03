using OmegaSudoku.Core;

namespace OmegaSudoku.Tests;

public class FileBoards
{
    [Fact]
    public void BoardsFromFile()
    {
        var parent = Directory.GetParent(Directory.GetCurrentDirectory());
        var projectDir = parent?.Parent?.Parent?.FullName;
        Assert.True(projectDir != null);
        var path = Path.Combine(projectDir, "boards.txt");;
        string[] lines = File.ReadAllLines(path);
        
        foreach (var line in lines)
        {
            // Arrange
            int[,] board = Board.FromString(line);
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