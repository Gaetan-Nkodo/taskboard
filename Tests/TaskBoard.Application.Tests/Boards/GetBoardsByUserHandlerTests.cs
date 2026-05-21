using FluentAssertions;
using NSubstitute;
using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Tests.Boards;

public class GetBoardsByUserHandlerTests
{
    private readonly IBoardRepository _boardRepository = Substitute.For<IBoardRepository>();

    [Fact]
    public async Task Handle_ShouldReturnBoardsForUser()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var boards = new List<Board>
        {
            new Board(userId, "Board 1", "Desc 1"),
            new Board(userId, "Board 2", "Desc 2")
        };

        _boardRepository.GetByUserIdAsync(userId).Returns(boards);

        var handler = new GetBoardsByUserHandler(_boardRepository);

        // Act
        var result = await handler.Handle(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(new { Name = "Board 1" });
        result.Should().ContainEquivalentOf(new { Name = "Board 2" });
    }
}
