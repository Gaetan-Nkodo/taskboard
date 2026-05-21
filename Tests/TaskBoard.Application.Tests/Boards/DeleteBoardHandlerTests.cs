using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Tests.Boards;

public class DeleteBoardHandlerTests
{
    private readonly IBoardRepository _boardRepository = Substitute.For<IBoardRepository>();

    [Fact]
    public async Task Handle_ShouldDeleteBoard_WhenUserOwnsBoard()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var board = new Board(userId, "Board");

        _boardRepository.GetByIdAsync(board.Id, userId).Returns(board);

        var handler = new DeleteBoardHandler(_boardRepository);

        // Act
        await handler.Handle(board.Id, userId);

        // Assert
        await _boardRepository.Received(1).DeleteAsync(board);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenBoardNotFound()
    {
        _boardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns((Board?)null);

        var handler = new DeleteBoardHandler(_boardRepository);

        var act = () => handler.Handle(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserDoesNotOwnBoard()
    {
        var board = new Board(Guid.NewGuid(), "Board"); // owner ≠ userId

        _boardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns(board);

        var handler = new DeleteBoardHandler(_boardRepository);

        var act = () => handler.Handle(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
