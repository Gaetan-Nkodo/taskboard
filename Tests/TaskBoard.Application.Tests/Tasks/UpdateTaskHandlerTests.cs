using FluentAssertions;
using NSubstitute;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Tasks;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;
using Xunit;

namespace TaskBoard.Application.Tests.Tasks;

public class UpdateTaskHandlerTests
{
    private readonly IBoardRepository _boardRepository = Substitute.For<IBoardRepository>();

    [Fact]
    public async Task Handle_ShouldUpdateTask_WhenTaskExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var board = new Board(userId, "Board");
        var column = board.AddColumn("Todo", 1);
        var task = column.AddTask("Old", "OldDesc", "OldIcon");

        _boardRepository.GetByTaskIdAsync(task.Id, userId).Returns(board);

        var request = new UpdateTaskRequest(column.Id, "New", "NewDesc", "NewIcon");

        var handler = new UpdateTaskHandler(_boardRepository);

        // Act
        await handler.Handle(task.Id, userId, request);

        // Assert
        task.Name.Should().Be("New");
        task.Description.Should().Be("NewDesc");
        task.Icon.Should().Be("NewIcon");

        await _boardRepository.Received(1).UpdateAsync(board);
    }

    [Fact]
    public async Task Handle_ShouldMoveTask_WhenColumnChanges()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var board = new Board(userId, "Board");

        var col1 = board.AddColumn("Todo", 1);
        var col2 = board.AddColumn("Done", 2);

        var task = col1.AddTask("Task", null, null);

        _boardRepository.GetByTaskIdAsync(task.Id, userId).Returns(board);

        var request = new UpdateTaskRequest(col2.Id, "Task", null, null);

        var handler = new UpdateTaskHandler(_boardRepository);

        // Act
        await handler.Handle(task.Id, userId, request);

        // Assert
        task.ColumnId.Should().Be(col2.Id);
        task.Order.Should().Be(1); // first task in new column

        await _boardRepository.Received(1).UpdateAsync(board);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenTaskNotFound()
    {
        // Arrange
        _boardRepository.GetByTaskIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns((Board?)null);

        var handler = new UpdateTaskHandler(_boardRepository);

        var request = new UpdateTaskRequest(Guid.NewGuid(), "Name", null, null);

        // Act
        var act = () => handler.Handle(Guid.NewGuid(), Guid.NewGuid(), request);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Task not found or access denied.");
    }
}
