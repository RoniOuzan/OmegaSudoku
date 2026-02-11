using OmegaSudoku.Core;
using Xunit.Abstractions;

namespace OmegaSudoku.Tests;

public class BasicBoardsTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public BasicBoardsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void AlreadySolvedBoard()
    {
        // Arrange
        int[,] board =
        {
            { 9, 8, 7, 6, 5, 4, 3, 2, 1 },
            { 2, 4, 6, 1, 7, 3, 9, 8, 5 },
            { 3, 5, 1, 9, 2, 8, 7, 4, 6 },
            { 1, 2, 8, 5, 3, 7, 6, 9, 4 },
            { 6, 3, 4, 8, 9, 2, 1, 5, 7 },
            { 7, 9, 5, 4, 6, 1, 8, 3, 2 },
            { 5, 1, 9, 2, 8, 6, 4, 7, 3 },
            { 4, 7, 2, 3, 1, 9, 5, 6, 8 },
            { 8, 6, 3, 7, 4, 5, 2, 1, 9 }
        };
        var original = (int[,])board.Clone();

        // Act
        (bool solved, long milliseconds) = Solver.TimedSolve(board);

        // Assert
        Assert.True(solved);
        Assert.True(Board.IsValidSudoku(board));
        Assert.Equal(original, board);
        Assert.True(milliseconds < 1000);
    }
    
    [Fact]
    public void EasyBoard()
    {
        // Arrange
        int[,] board =
        {
            { 5, 3, 0, 0, 7, 0, 0, 0, 0 },
            { 6, 0, 0, 1, 9, 5, 0, 0, 0 },
            { 0, 9, 8, 0, 0, 0, 0, 6, 0 },
            { 8, 0, 0, 0, 6, 0, 0, 0, 3 },
            { 4, 0, 0, 8, 0, 3, 0, 0, 1 },
            { 7, 0, 0, 0, 2, 0, 0, 0, 6 },
            { 0, 6, 0, 0, 0, 0, 2, 8, 0 },
            { 0, 0, 0, 4, 1, 9, 0, 0, 5 },
            { 0, 0, 0, 0, 8, 0, 0, 7, 9 }
        };

        // Act
        (bool solved, long milliseconds) = Solver.TimedSolve(board);

        // Assert
        Assert.True(solved);
        Assert.True(Board.IsValidSudoku(board));
        Assert.True(milliseconds < 1000);
    }
    
    [Fact]
    public void EmptyBoard()
    {
        // Arrange
        var board = new int[9, 9];

        // Act
        (bool solved, long milliseconds) = Solver.TimedSolve(board);

        // Assert
        Assert.True(solved);
        Assert.True(Board.IsValidSudoku(board));
        Assert.True(milliseconds < 1000);
    }
    
    [Fact]
    public void VeryComplexBoard()
    {
        // Arrange
        int[,] board =
        {
            { 0, 8, 0, 5, 0, 0, 0, 0, 2 },
            { 3, 0, 0, 6, 0, 0, 0, 4, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 4, 2, 0, 0, 8, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 6, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 4, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 3, 2, 0, 0 },
            { 7, 0, 0, 0, 0, 0, 0, 0, 8 }
        };

        // Act
        (bool solved, long milliseconds) = Solver.TimedSolve(board);

        // Assert
        Assert.True(solved);
        Assert.True(Board.IsValidSudoku(board));
        Assert.True(milliseconds < 1000);

        _testOutputHelper.WriteLine(milliseconds.ToString());
    }
}