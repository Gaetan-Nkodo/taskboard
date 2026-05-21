using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.UseCases.Tasks;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Tests.Tasks;

public class DeleteTaskHandlerTests
{
    private readonly IBoardRepository _boardRepository = Substitute.For<IBoardRepository>();

    [Fact]
    public async Task Handle_ShouldDeleteTask_WhenTaskExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var board = new Board(userId, "Board");
        var column = board.AddColumn("Todo", 1);
        var task = column.AddTask("Task", null, null);

        _boardRepository.GetByTaskIdAsync(task.Id, userId).Returns(board);

        var handler = new DeleteTaskHandler(_boardRepository);

        // Act
        await handler.Handle(task.Id, userId);

        // Assert
        column.Tasks.Should().BeEmpty();
        await _boardRepository.Received(1).UpdateAsync(board);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenTaskNotFound()
    {
        // Arrange
        _boardRepository.GetByTaskIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns((Board?)null);

        var handler = new DeleteTaskHandler(_boardRepository);

        // Act
        var act = () => handler.Handle(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserDoesNotOwnBoard()
    {
        // Arrange
        var board = new Board(Guid.NewGuid(), "Board"); // owner ≠ userId
        var userId = Guid.NewGuid();

        _boardRepository.GetByTaskIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns(board);

        var handler = new DeleteTaskHandler(_boardRepository);

        // Act
        var act = () => handler.Handle(Guid.NewGuid(), userId);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
