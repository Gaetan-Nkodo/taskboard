using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.DTOs;
using TaskBoard.Application.UseCases.Tasks;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Tasks;

public class GetTasksByBoardHandlerTests
{
    [Fact]
    public async Task Handle_ShouldMapTasksFromRepositoryToDto()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var columnId = Guid.NewGuid();

        var task = new TaskItem(columnId, "Task", "Desc", "🔥", 1);

        var repo = Substitute.For<ITaskRepository>();
        repo.GetByBoardAsync(boardId, userId)
            .Returns(new List<TaskItem> { task });

        var handler = new GetTasksByBoardHandler(repo);

        // Act
        var result = await handler.Handle(boardId, userId);

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Task");
        result[0].ColumnId.Should().Be(columnId);
    }
}
