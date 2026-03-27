using NSubstitute;
using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Boards;

public class GetBoardHandlerTests
{
    [Fact]
    public async Task Should_Return_Board_When_User_Owns_It()
    {
        var repo = Substitute.For<IBoardRepository>();
        var handler = new GetBoardHandler(repo);

        var userId = Guid.NewGuid();
        var board = new Board(userId, "Board");

        repo.GetByIdAsync(board.Id).Returns(board);

        var result = await handler.Handle(board.Id);

        Assert.Equal(board.Id, result);
        Assert.Equal("Board", result);
    }

    [Fact]
    public async Task Should_Throw_When_Board_Not_Found()
    {
        var repo = Substitute.For<IBoardRepository>();
        var handler = new GetBoardHandler(repo);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(Guid.NewGuid()));
    }

    [Fact]
    public async Task Should_Throw_When_User_Does_Not_Own_Board()
    {
        var repo = Substitute.For<IBoardRepository>();
        var handler = new GetBoardHandler(repo);

        var board = new Board(Guid.NewGuid(), "Board");
        repo.GetByIdAsync(board.Id).Returns(board);

        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(board.Id));
    }
}
