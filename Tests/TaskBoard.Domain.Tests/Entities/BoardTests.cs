using FluentAssertions;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Domain.Tests.Entities;

public class BoardTests
{
    [Fact]
    public void Constructor_ShouldInitializeBoardCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "My Board";
        var description = "Board description";

        // Act
        var board = new Board(userId, name, description);

        // Assert
        board.Id.Should().NotBe(Guid.Empty);
        board.UserId.Should().Be(userId);
        board.Name.Should().Be(name);
        board.Description.Should().Be(description);
        board.Columns.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldAllowNullDescription()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Board";

        // Act
        var board = new Board(userId, name);

        // Assert
        board.Description.Should().BeNull();
    }

    [Fact]
    public void AddColumn_ShouldAddColumnToBoard()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Board");

        // Act
        var column = board.AddColumn("Todo", 1);

        // Assert
        board.Columns.Should().HaveCount(1);
        board.Columns.First().Name.Should().Be("Todo");
        board.Columns.First().Order.Should().Be(1);
        board.Columns.First().BoardId.Should().Be(board.Id);

        column.Should().NotBeNull();
    }

    [Fact]
    public void Update_ShouldModifyNameAndDescription()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Old Name", "Old Desc");

        // Act
        board.Update("New Name", "New Desc");

        // Assert
        board.Name.Should().Be("New Name");
        board.Description.Should().Be("New Desc");
    }

    [Fact]
    public void Update_ShouldAllowNullDescription()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Old Name", "Old Desc");

        // Act
        board.Update("New Name", null);

        // Assert
        board.Name.Should().Be("New Name");
        board.Description.Should().BeNull();
    }
}
