using FluentAssertions;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Domain.Tests.Entities;

public class ColumnTests
{
    [Fact]
    public void Constructor_ShouldInitializeColumnCorrectly()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var name = "Todo";
        var order = 1;

        // Act
        var column = new Column(boardId, name, order);

        // Assert
        column.Id.Should().NotBe(Guid.Empty);
        column.BoardId.Should().Be(boardId);
        column.Name.Should().Be(name);
        column.Order.Should().Be(order);
        column.Tasks.Should().BeEmpty();
    }

    [Fact]
    public void AddTask_ShouldAddTaskWithAutoOrder_WhenOrderIsNull()
    {
        // Arrange
        var column = new Column(Guid.NewGuid(), "Todo", 1);

        // Act
        var task1 = column.AddTask("Task 1");
        var task2 = column.AddTask("Task 2");

        // Assert
        task1.Order.Should().Be(1);
        task2.Order.Should().Be(2);
        column.Tasks.Should().HaveCount(2);
    }

    [Fact]
    public void AddTask_ShouldUseProvidedOrder_WhenOrderIsSpecified()
    {
        // Arrange
        var column = new Column(Guid.NewGuid(), "Todo", 1);

        // Act
        var task = column.AddTask("Task 1", "desc", "icon", 10);

        // Assert
        task.Order.Should().Be(10);
    }

    [Fact]
    public void RemoveTask_ShouldRemoveTask_WhenTaskExists()
    {
        // Arrange
        var column = new Column(Guid.NewGuid(), "Todo", 1);
        var task = column.AddTask("Task 1");

        // Act
        column.RemoveTask(task.Id);

        // Assert
        column.Tasks.Should().BeEmpty();
    }

    [Fact]
    public void RemoveTask_ShouldDoNothing_WhenTaskDoesNotExist()
    {
        // Arrange
        var column = new Column(Guid.NewGuid(), "Todo", 1);
        column.AddTask("Task 1");

        // Act
        column.RemoveTask(Guid.NewGuid());

        // Assert
        column.Tasks.Should().HaveCount(1);
    }
}
