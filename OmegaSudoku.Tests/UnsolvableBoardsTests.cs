using OmegaSudoku.Core;

namespace OmegaSudoku.Tests;

public class UnsolvableBoardsTests
{
    [Fact]
    public void DuplicateInRow()
    {
        // Arrange
        int[,] board =
        {
            { 5, 5, 0, 0, 7, 0, 0, 0, 0 },
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
        Assert.False(solved);
        Assert.True(milliseconds < 1000);
    }

    [Fact]
    public void ImpossibleCell()
    {
        // Arrange
        int[,] board =
        {
            { 1, 2, 3, 4, 5, 6, 7, 8, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 9 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        // Act
        (bool solved, long milliseconds) = Solver.TimedSolve(board);

        // Assert
        Assert.False(solved);
        Assert.True(milliseconds < 1000);
    }
    
    [Fact]
    public void ImpossibleLastCell()
    {
        // Arrange
        int[,] board =
        {
            { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
            { 7, 8, 9, 1, 2, 3, 4, 5, 6 },
            { 2, 3, 4, 5, 6, 7, 8, 9, 1 },
            { 5, 6, 7, 8, 9, 1, 2, 3, 4 },
            { 8, 9, 1, 2, 3, 4, 5, 6, 7 },
            { 3, 4, 5, 6, 7, 8, 9, 1, 2 },
            { 6, 7, 8, 9, 1, 2, 3, 4, 5 },
            { 9, 1, 2, 3, 4, 5, 6, 7, 0 }
        };

        // Act
        (bool solved, long milliseconds) = Solver.TimedSolve(board);

        // Assert
        Assert.False(solved);
        Assert.True(milliseconds < 1000);
    }
}