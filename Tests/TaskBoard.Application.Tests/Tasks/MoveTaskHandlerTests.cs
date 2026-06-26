using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Tasks;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Tasks;

public class MoveTaskHandlerTests
{
    private readonly ITaskRepository _taskRepository = Substitute.For<ITaskRepository>();

    [Fact]
    public async Task Handle_ShouldMoveTaskWithinSameColumn_AndRecalculateOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var columnId = Guid.NewGuid();

        var task1 = new TaskItem(columnId, "T1", null, null, 0);
        var task2 = new TaskItem(columnId, "T2", null, null, 1);
        var task3 = new TaskItem(columnId, "T3", null, null, 2);

        _taskRepository.GetByIdAsync(task2.Id, userId).Returns(task2);
        _taskRepository.GetByColumnAsync(columnId, userId)
            .Returns(new List<TaskItem> { task1, task2, task3 });

        var handler = new MoveTaskHandler(_taskRepository);

        var request = new MoveTaskRequest(columnId, 0); // move T2 to index 0

        // Act
        await handler.Handle(task2.Id, userId, request);

        // Assert
        task2.Order.Should().Be(0);
        task1.Order.Should().Be(1);
        task3.Order.Should().Be(2);

        await _taskRepository.Received(1).SaveAllAsync(Arg.Any<IEnumerable<TaskItem>>());
    }

    [Fact]
    public async Task Handle_ShouldMoveTaskToAnotherColumn_AndRecalculateOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var colA = Guid.NewGuid();
        var colB = Guid.NewGuid();

        var taskA1 = new TaskItem(colA, "A1", null, null, 0);
        var taskA2 = new TaskItem(colA, "A2", null, null, 1);

        var taskToMove = taskA2;

        var taskB1 = new TaskItem(colB, "B1", null, null, 0);

        _taskRepository.GetByIdAsync(taskToMove.Id, userId)
            .Returns(taskToMove);

        _taskRepository.GetByColumnAsync(colA, userId)
            .Returns(new List<TaskItem> { taskA1, taskA2 });

        _taskRepository.GetByColumnAsync(colB, userId)
            .Returns(new List<TaskItem> { taskB1 });

        var handler = new MoveTaskHandler(_taskRepository);

        var request = new MoveTaskRequest(colB, 1); // insert at index 1

        // Act
        await handler.Handle(taskToMove.Id, userId, request);

        // Assert
        taskToMove.ColumnId.Should().Be(colB);
        taskToMove.Order.Should().Be(1);

        taskB1.Order.Should().Be(0);

        await _taskRepository.Received(1)
            .SaveAllAsync(Arg.Any<IEnumerable<TaskItem>>());
    }


    [Fact]
    public async Task Handle_ShouldThrow_WhenTaskNotFound()
    {
        // Arrange
        _taskRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns((TaskItem?)null);

        var handler = new MoveTaskHandler(_taskRepository);

        var request = new MoveTaskRequest(Guid.NewGuid(), 0);

        // Act
        var act = () => handler.Handle(Guid.NewGuid(), Guid.NewGuid(), request);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Task not found or not accessible.");
    }
}
