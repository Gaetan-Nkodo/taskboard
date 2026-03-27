using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;

namespace TaskBoard.Domain.Tests.Entities;

public class TaskItemTests
{
    [Fact]
    public void Should_Create_TaskItem()
    {
        var boardId = Guid.NewGuid();

        var task = new TaskItem(boardId, "Task", "Desc", "📌", "Todo");

        Assert.Equal(boardId, task.BoardId);
        Assert.Equal("Task", task.Name);
        Assert.Equal("Desc", task.Description);
        Assert.Equal("📌", task.Icon);
        Assert.Equal("Todo", task.Status);
    }

    [Fact]
    public void Should_Update_TaskItem()
    {
        var boardId = Guid.NewGuid();
        var task = new TaskItem(boardId, "Old", "Old Desc", "📌", "Todo");

        task.Update("New", "New Desc", "⭐", "Done");

        Assert.Equal("New", task.Name);
        Assert.Equal("New Desc", task.Description);
        Assert.Equal("⭐", task.Icon);
        Assert.Equal("Done", task.Status);
    }

    [Fact]
    public void Should_Throw_When_Updating_With_Invalid_Data()
    {
        var boardId = Guid.NewGuid();
        var task = new TaskItem(boardId, "Task", "Desc", "📌", "Todo");

        Assert.Throws<DomainException>(() =>
            task.Update("", "Desc", "📌", "Todo"));
    }
}
