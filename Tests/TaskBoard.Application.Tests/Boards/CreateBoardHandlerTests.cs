using NSubstitute;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Boards;

public class CreateBoardHandlerTests
{
    [Fact]
    public async Task Should_Create_Board()
    {
        var repo = Substitute.For<IBoardRepository>();
        var handler = new CreateBoardHandler(repo);

        var request = new CreateBoardRequest("My Board", "Description");

        var userId = Guid.NewGuid();

        var id = await handler.Handle(userId, request);

        await repo.Received(1).AddAsync(Arg.Any<Board>());
        Assert.NotEqual(Guid.Empty, id);
    }
}
