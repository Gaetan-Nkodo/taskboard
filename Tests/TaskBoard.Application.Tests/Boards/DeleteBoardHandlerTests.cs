using NSubstitute;
using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Boards;

public class DeleteBoardHandlerTests
{
    [Fact]
    public async Task Should_Delete_Board_When_User_Owns_It()
    {
        var repo = Substitute.For<IBoardRepository>();
        var handler = new DeleteBoardHandler(repo);

        var userId = Guid.NewGuid();
        var board = new Board(userId, "Board");

        repo.GetByIdAsync(board.Id, userId).Returns(board);

        await handler.Handle(board.Id, userId);

        await repo.Received(1).DeleteAsync(board);
    }

    [Fact]
    public async Task Should_Throw_When_Board_Not_Found()
    {
        var repo = Substitute.For<IBoardRepository>();
        var handler = new DeleteBoardHandler(repo);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task Should_Throw_When_User_Does_Not_Own_Board()
    {
        var repo = Substitute.For<IBoardRepository>();
        var handler = new DeleteBoardHandler(repo);

        var board = new Board(Guid.NewGuid(), "Board");
        repo.GetByIdAsync(board.Id, Guid.Empty).Returns(board);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            handler.Handle(board.Id, Guid.NewGuid()));
    }
}
