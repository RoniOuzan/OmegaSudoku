using OmegaSudoku.Core;

namespace OmegaSudoku.Tests;

public class UnsolvableBoards
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
        var (result, milliseconds) = Solver.TimedSolve(board);

        // Assert
        Assert.False(result);
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
        var (result, milliseconds) = Solver.TimedSolve(board);

        // Assert
        Assert.False(result);
        Assert.True(milliseconds < 1000);
    }
}