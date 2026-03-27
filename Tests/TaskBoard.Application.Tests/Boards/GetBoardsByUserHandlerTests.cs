using NSubstitute;
using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Boards;

public class GetBoardsByUserHandlerTests
{
    [Fact]
    public async Task Should_Return_All_Boards_For_User()
    {
        var repo = Substitute.For<IBoardRepository>();
        var handler = new GetBoardsByUserHandler(repo);

        var userId = Guid.NewGuid();

        var boards = new List<Board>
        {
            new Board(userId, "Board 1"),
            new Board(userId, "Board 2")
        };

        repo.GetByUserIdAsync(userId).Returns(boards);

        var result = await handler.Handle(userId);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Should_Return_Empty_List_When_User_Has_No_Boards()
    {
        var repo = Substitute.For<IBoardRepository>();
        var handler = new GetBoardsByUserHandler(repo);

        var userId = Guid.NewGuid();

        repo.GetByUserIdAsync(userId).Returns(new List<Board>());

        var result = await handler.Handle(userId);

        Assert.Empty(result);
    }
}
