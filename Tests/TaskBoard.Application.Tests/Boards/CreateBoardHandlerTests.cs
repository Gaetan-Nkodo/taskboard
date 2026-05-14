using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;
using Moq;
using FluentAssertions;

public class CreateBoardHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateBoardWithDefaultColumnsAndTasks()
    {
        var repo = new Mock<IBoardRepository>();
        var handler = new CreateBoardHandler(repo.Object);

        var request = new CreateBoardRequest("Board", "Desc");
        var userId = Guid.NewGuid();

        var boardId = await handler.Handle(userId, request);

        repo.Verify(r => r.AddAsync(It.Is<Board>(b =>
            b.Columns.Count == 3 &&
            b.Columns.Any(c => c.Name == "In Progress") &&
            b.Columns.Any(c => c.Name == "Completed") &&
            b.Columns.Any(c => c.Name == "Won't Do")
        )), Times.Once);
    }
}
