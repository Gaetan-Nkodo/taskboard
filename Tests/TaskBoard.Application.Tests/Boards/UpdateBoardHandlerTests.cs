using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Tests.Boards;

public class UpdateBoardHandlerTests
{
    private readonly IBoardRepository _boardRepository = Substitute.For<IBoardRepository>();

    [Fact]
    public async Task Handle_ShouldUpdateBoard_WhenUserOwnsBoard()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var board = new Board(userId, "Old", "OldDesc");

        _boardRepository.GetByIdAsync(board.Id, userId).Returns(board);

        var request = new UpdateBoardRequest("New", "NewDesc");

        var handler = new UpdateBoardHandler(_boardRepository);

        // Act
        await handler.Handle(board.Id, userId, request);

        // Assert
        board.Name.Should().Be("New");
        board.Description.Should().Be("NewDesc");

        await _boardRepository.Received(1).UpdateAsync(board);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenBoardNotFound()
    {
        _boardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns((Board?)null);

        var handler = new UpdateBoardHandler(_boardRepository);

        var act = () => handler.Handle(Guid.NewGuid(), Guid.NewGuid(), new UpdateBoardRequest("A", null));

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserDoesNotOwnBoard()
    {
        var board = new Board(Guid.NewGuid(), "Board");

        _boardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns(board);

        var handler = new UpdateBoardHandler(_boardRepository);

        var act = () => handler.Handle(Guid.NewGuid(), Guid.NewGuid(), new UpdateBoardRequest("A", null));

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
