using NSubstitute;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Boards;

public class UpdateBoardHandlerTests
{
    [Fact]
    public async Task Should_Update_Board_When_User_Owns_It()
    {
        var repo = Substitute.For<IBoardRepository>();
        var handler = new UpdateBoardHandler(repo);

        var userId = Guid.NewGuid();
        var board = new Board(userId, "Old Name", "Old Desc");

        repo.GetByIdAsync(board.Id, userId).Returns(board);

        var request = new UpdateBoardRequest("New Name", "New Desc");

        await handler.Handle(board.Id, userId, request);

        await repo.Received(1).UpdateAsync(board);
        Assert.Equal("New Name", board.Name);
        Assert.Equal("New Desc", board.Description);
    }

    [Fact]
    public async Task Should_Throw_When_Board_Not_Found()
    {
        var repo = Substitute.For<IBoardRepository>();
        var handler = new UpdateBoardHandler(repo);

        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns((Board?)null);

        var request = new UpdateBoardRequest("Name", "Desc");

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(Guid.NewGuid(), Guid.NewGuid(), request));
    }
}
