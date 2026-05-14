using TaskBoard.Application.UseCases.Tasks;
using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;
using Moq;
using FluentAssertions;

public class CreateTaskHandlerTests
{
    [Fact]
    public async Task Handle_ShouldAddTaskToColumn()
    {
        var board = new Board(Guid.NewGuid(), "Board", null);
        var column = board.AddColumn("In Progress", 1);

        var repo = new Mock<IBoardRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(board);

        var handler = new CreateTaskHandler(repo.Object);

        var request = new CreateTaskRequest(column.Id, "Task", "Desc", "🔥");

        var taskId = await handler.Handle(board.Id, board.UserId, request);

        column.Tasks.Should().ContainSingle(t => t.Id == taskId);
    }
}
