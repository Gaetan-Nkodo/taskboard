using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Tasks;

public class CreateTaskHandlerTests
{
    private readonly IBoardRepository _boardRepository = Substitute.For<IBoardRepository>();
    private readonly ITaskRepository _taskRepository = Substitute.For<ITaskRepository>();

    [Fact]
    public async Task Handle_ShouldCreateTask_WhenBoardAndColumnExist()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var columnId = Guid.NewGuid();

        var board = new Board(userId, "Board");
        var column = board.AddColumn("Todo", 1);

        // On force l'ID de la colonne
        typeof(Column).GetProperty(nameof(Column.Id))!.SetValue(column, columnId);

        _boardRepository.GetByIdAsync(boardId, userId).Returns(board);

        var request = new CreateTaskRequest(columnId, "Task", "Desc", "🔥");

        var handler = new CreateTaskHandler(_boardRepository, _taskRepository);

        // Act
        var taskId = await handler.Handle(boardId, userId, request);

        // Assert
        taskId.Should().NotBe(Guid.Empty);

        // 🔥 Vérifie que la Task est bien envoyée au repository
        await _taskRepository.Received(1).AddAsync(
            Arg.Is<TaskItem>(t =>
                t.Name == "Task" &&
                t.Description == "Desc" &&
                t.Icon == "🔥" &&
                t.ColumnId == columnId
            )
        );

        // 🔥 Vérifie qu'on NE met plus à jour le board
        await _boardRepository.DidNotReceive().UpdateAsync(Arg.Any<Board>());
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenBoardNotFound()
    {
        var handler = new CreateTaskHandler(_boardRepository, _taskRepository);

        _boardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns((Board?)null);

        var request = new CreateTaskRequest(Guid.NewGuid(), "Task", null, null);

        var act = () => handler.Handle(Guid.NewGuid(), Guid.NewGuid(), request);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Board not found or access denied.");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenColumnNotFound()
    {
        var userId = Guid.NewGuid();
        var board = new Board(userId, "Board");

        _boardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns(board);

        var request = new CreateTaskRequest(Guid.NewGuid(), "Task", null, null);

        var handler = new CreateTaskHandler(_boardRepository, _taskRepository);

        var act = () => handler.Handle(Guid.NewGuid(), userId, request);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Column not found.");
    }
}
