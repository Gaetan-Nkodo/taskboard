using FluentAssertions;
using TaskBoard.Domain.Entities;
using Xunit;

namespace TaskBoard.Domain.Tests.Entities;

public class TaskItemTests
{
    [Fact]
    public void Constructor_ShouldInitializeTaskCorrectly()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var name = "Task";
        var description = "Desc";
        var icon = "🔥";
        var order = 3;

        // Act
        var task = new TaskItem(columnId, name, description, icon, order);

        // Assert
        task.Id.Should().NotBe(Guid.Empty);
        task.ColumnId.Should().Be(columnId);
        task.Name.Should().Be(name);
        task.Description.Should().Be(description);
        task.Icon.Should().Be(icon);
        task.Order.Should().Be(order);
    }

    [Fact]
    public void Update_ShouldModifyNameDescriptionAndIcon()
    {
        // Arrange
        var task = new TaskItem(Guid.NewGuid(), "Old", "OldDesc", "OldIcon", 1);

        // Act
        task.Update("New", "NewDesc", "NewIcon");

        // Assert
        task.Name.Should().Be("New");
        task.Description.Should().Be("NewDesc");
        task.Icon.Should().Be("NewIcon");
    }

    [Fact]
    public void MoveToColumn_ShouldUpdateColumnIdAndOrder()
    {
        // Arrange
        var task = new TaskItem(Guid.NewGuid(), "Task", "Desc", "Icon", 1);
        var newColumnId = Guid.NewGuid();

        // Act
        task.MoveToColumn(newColumnId, 5);

        // Assert
        task.ColumnId.Should().Be(newColumnId);
        task.Order.Should().Be(5);
    }
}
