using NSubstitute;
using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Domain.Entities;

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

        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var board = new Board(ownerId, "Board");

        // Le repo renvoie bien le board pour le propriétaire
        repo.GetByIdAsync(board.Id, ownerId).Returns(board);

        // Mais on appelle le handler avec un autre userId
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            handler.Handle(board.Id, otherUserId));
    }
}
