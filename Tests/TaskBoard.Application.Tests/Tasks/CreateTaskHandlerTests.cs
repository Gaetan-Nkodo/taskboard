using NSubstitute;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Tasks;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Tasks;

public class CreateTaskHandlerTests
{
    [Fact]
    public async Task Should_Create_Task()
    {
        var boardRepo = Substitute.For<IBoardRepository>();
        var taskRepo = Substitute.For<ITaskRepository>();

        var handler = new CreateTaskHandler(boardRepo, taskRepo);

        var board = new Board(Guid.NewGuid(), "Board");
        boardRepo.GetByIdAsync(board.Id).Returns(board);

        var request = new CreateTaskRequest("Task", "Desc", "📌", "Todo");

        var id = await handler.Handle(board.Id, request);

        await taskRepo.Received(1).AddAsync(Arg.Any<TaskItem>());
    }
}
