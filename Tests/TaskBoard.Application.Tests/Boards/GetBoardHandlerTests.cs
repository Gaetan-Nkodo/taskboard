using FluentAssertions;
using NSubstitute;
using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Tests.Boards;

public class GetBoardHandlerTests
{
    private readonly IBoardRepository _boardRepository = Substitute.For<IBoardRepository>();

    [Fact]
    public async Task Handle_ShouldReturnBoardDto_WhenBoardExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var boardId = Guid.NewGuid();

        var board = new Board(userId, "Board", "Desc");
        var col = board.AddColumn("Todo", 1);
        col.AddTask("Task 1");

        _boardRepository.GetByIdAsync(boardId, userId).Returns(board);

        var handler = new GetBoardHandler(_boardRepository);

        // Act
        var dto = await handler.Handle(boardId, userId);

        // Assert
        dto.Id.Should().Be(board.Id);
        dto.Name.Should().Be("Board");
        dto.Columns.Should().HaveCount(1);
        dto.Columns.First().Tasks.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenBoardNotFound()
    {
        // Arrange
        _boardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns((Board?)null);

        var handler = new GetBoardHandler(_boardRepository);

        // Act
        var act = () => handler.Handle(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
