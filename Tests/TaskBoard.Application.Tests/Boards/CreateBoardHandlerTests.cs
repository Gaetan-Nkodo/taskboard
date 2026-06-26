using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Tests.Boards;

public class CreateBoardHandlerTests
{
    private readonly IBoardRepository _boardRepository = Substitute.For<IBoardRepository>();

    [Fact]
    public async Task Handle_ShouldCreateBoardWithDefaultColumns()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateBoardRequest("My Board", "Desc");

        var handler = new CreateBoardHandler(_boardRepository);

        // Act
        var boardId = await handler.Handle(userId, request);

        // Assert
        boardId.Should().NotBe(Guid.Empty);

        await _boardRepository.Received(1)
            .AddAsync(Arg.Is<Board>(b =>
                b.UserId == userId &&
                b.Name == "My Board" &&
                b.Columns.Count == 5 &&
                b.Columns.Any(c => c.Name == "Backlog") &&
                b.Columns.Any(c => c.Name == "Ready") &&
                b.Columns.Any(c => c.Name == "In Progress") &&
                b.Columns.Any(c => c.Name == "Review") &&
                b.Columns.Any(c => c.Name == "Done") &&
                b.Columns.SelectMany(c => c.Tasks).Count() == 0
            ));
    }
}
