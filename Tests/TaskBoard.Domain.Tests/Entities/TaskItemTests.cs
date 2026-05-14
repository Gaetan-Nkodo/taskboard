using TaskBoard.Domain.Entities;
using FluentAssertions;

public class TaskItemTests
{
    [Fact]
    public void Update_ShouldModifyTaskProperties()
    {
        var task = new TaskItem(Guid.NewGuid(), "Old", "OldDesc", "🔥", 1);

        task.Update("New", "NewDesc", "⭐");

        task.Name.Should().Be("New");
        task.Description.Should().Be("NewDesc");
        task.Icon.Should().Be("⭐");
    }

    [Fact]
    public void MoveToColumn_ShouldUpdateColumnIdAndOrder()
    {
        var task = new TaskItem(Guid.NewGuid(), "Task", null, null, 1);

        var newColumnId = Guid.NewGuid();
        task.MoveToColumn(newColumnId, 3);

        task.ColumnId.Should().Be(newColumnId);
        task.Order.Should().Be(3);
    }
}
