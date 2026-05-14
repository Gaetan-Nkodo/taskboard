using TaskBoard.Application.UseCases.Tasks;
using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;
using Moq;
using FluentAssertions;

public class UpdateTaskHandlerTests
{
    [Fact]
    public async Task Handle_ShouldUpdateTaskProperties()
    {
        var board = new Board(Guid.NewGuid(), "Board", null);
        var column = board.AddColumn("In Progress", 1);
        var task = column.AddTask("Old", "OldDesc", "🔥");

        var repo = new Mock<IBoardRepository>();
        repo.Setup(r => r.GetByTaskIdAsync(task.Id, board.UserId))
            .ReturnsAsync(board);

        var handler = new UpdateTaskHandler(repo.Object);

        var request = new UpdateTaskRequest(column.Id, "New", "NewDesc", "⭐");

        await handler.Handle(task.Id, board.UserId, request);

        task.Name.Should().Be("New");
        task.Description.Should().Be("NewDesc");
        task.Icon.Should().Be("⭐");
    }

    [Fact]
    public async Task Handle_ShouldMoveTaskToAnotherColumn()
    {
        var board = new Board(Guid.NewGuid(), "Board", null);
        var col1 = board.AddColumn("In Progress", 1);
        var col2 = board.AddColumn("Completed", 2);

        var task = col1.AddTask("Task");

        var repo = new Mock<IBoardRepository>();
        repo.Setup(r => r.GetByTaskIdAsync(task.Id, board.UserId))
            .ReturnsAsync(board);

        var handler = new UpdateTaskHandler(repo.Object);

        var request = new UpdateTaskRequest(col2.Id, "Task", null, null);

        await handler.Handle(task.Id, board.UserId, request);

        task.ColumnId.Should().Be(col2.Id);
    }
}
