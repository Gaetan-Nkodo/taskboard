using TaskBoard.Application.UseCases.Tasks;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;
using Moq;
using FluentAssertions;

public class DeleteTaskHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRemoveTaskFromColumn()
    {
        var board = new Board(Guid.NewGuid(), "Board", null);
        var column = board.AddColumn("In Progress", 1);
        var task = column.AddTask("Task");

        var repo = new Mock<IBoardRepository>();
        repo.Setup(r => r.GetByTaskIdAsync(task.Id, board.UserId))
            .ReturnsAsync(board);

        var handler = new DeleteTaskHandler(repo.Object);

        await handler.Handle(task.Id, board.UserId);

        column.Tasks.Should().BeEmpty();
    }
}
