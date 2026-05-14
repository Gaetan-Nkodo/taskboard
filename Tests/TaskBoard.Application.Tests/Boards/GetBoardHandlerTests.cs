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

        repo.GetByIdAsync(board.Id, userId).Returns(board);

        var result = await handler.Handle(board.Id, userId);

        Assert.Equal(board.Id, result.Id);
        Assert.Equal("Board", result.Name);
    }

    [Fact]
    public async Task Should_Throw_When_Board_Not_Found()
    {
        var repo = Substitute.For<IBoardRepository>();
        var handler = new GetBoardHandler(repo);

        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns((Board?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(Guid.NewGuid(), Guid.NewGuid()));
    }
}
