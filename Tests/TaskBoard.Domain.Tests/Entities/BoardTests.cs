using TaskBoard.Domain.Entities;

public class BoardTests
{
    [Fact]
    public void CreateBoard_ShouldSetProperties()
    {
        var board = new Board(Guid.NewGuid(), "Test Board");

        Assert.Equal("Test Board", board.Name);
        Assert.Empty(board.Tasks);
    }
}