using TaskBoard.Domain.Entities;
using FluentAssertions;

public class ColumnTests
{
    [Fact]
    public void AddTask_ShouldAddTaskWithCorrectOrder()
    {
        var column = new Column(Guid.NewGuid(), "In Progress", 1);

        var task1 = column.AddTask("Task 1");
        var task2 = column.AddTask("Task 2");

        task1.Order.Should().Be(1);
        task2.Order.Should().Be(2);
    }
}