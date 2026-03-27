using NSubstitute;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Tasks;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Tasks;

public class UpdateTaskHandlerTests
{
    [Fact]
    public async Task Should_Update_Task()
    {
        var repo = Substitute.For<ITaskRepository>();
        var handler = new UpdateTaskHandler(repo);

        var task = new TaskItem(Guid.NewGuid(), "Old", "Old Desc", "📌", "Todo");

        repo.GetByIdAsync(task.Id).Returns(task);

        var request = new UpdateTaskRequest("New", "New Desc", "⭐", "Done");

        await handler.Handle(task.Id, request);

        await repo.Received(1).UpdateAsync(task);

        Assert.Equal("New", task.Name);
        Assert.Equal("New Desc", task.Description);
        Assert.Equal("⭐", task.Icon);
        Assert.Equal("Done", task.Status);
    }

    [Fact]
    public async Task Should_Throw_When_Task_Not_Found()
    {
        var repo = Substitute.For<ITaskRepository>();
        var handler = new UpdateTaskHandler(repo);

        var request = new UpdateTaskRequest("Name", "Desc", "📌", "Todo");

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(Guid.NewGuid(), request));
    }
}
