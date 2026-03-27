using NSubstitute;
using TaskBoard.Application.UseCases.Tasks;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Tasks;

public class DeleteTaskHandlerTests
{
    [Fact]
    public async Task Should_Delete_Task()
    {
        var repo = Substitute.For<ITaskRepository>();
        var handler = new DeleteTaskHandler(repo);

        var task = new TaskItem(Guid.NewGuid(), "Task", "Desc", "📌", "Todo");

        repo.GetByIdAsync(task.Id).Returns(task);

        await handler.Handle(task.Id);

        await repo.Received(1).DeleteAsync(task);
    }

    [Fact]
    public async Task Should_Throw_When_Task_Not_Found()
    {
        var repo = Substitute.For<ITaskRepository>();
        var handler = new DeleteTaskHandler(repo);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(Guid.NewGuid()));
    }
}
