using TaskBoard.Domain.Entities;
using FluentAssertions;

public class BoardTests
{
    [Fact]
    public void CreateBoard_ShouldInitializeCorrectly()
    {
        var board = new Board(Guid.NewGuid(), "My Board", "Desc");

        board.Name.Should().Be("My Board");
        board.Description.Should().Be("Desc");
        board.Columns.Should().BeEmpty();
    }

    [Fact]
    public void AddColumn_ShouldAddColumnToBoard()
    {
        var board = new Board(Guid.NewGuid(), "Board", null);

        var column = board.AddColumn("In Progress", 1);

        board.Columns.Should().Contain(column);
        column.Name.Should().Be("In Progress");
        column.Order.Should().Be(1);
    }
}
