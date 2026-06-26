using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Persistence;
using TaskBoard.Infrastructure.Persistence.Repositories;

namespace TaskBoard.Infrastructure.Tests.Persistence;

public class TaskRepositoryTests
{
    private AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByBoardAsync_ShouldReturnOnlyTasksOfUserBoard()
    {
        var userId = Guid.NewGuid();
        var boardId = Guid.NewGuid();

        using var db = CreateDb();

        var board = new Board(userId, "Board");
        typeof(Board).GetProperty(nameof(Board.Id))!.SetValue(board, boardId);

        var col = board.AddColumn("Todo", 1);
        db.Boards.Add(board);

        db.Tasks.Add(new TaskItem(col.Id, "T1", "D1", "🔥", 1));
        db.Tasks.Add(new TaskItem(col.Id, "T2", "D2", "🔥", 2));

        var otherBoard = new Board(Guid.NewGuid(), "Other");
        var otherCol = otherBoard.AddColumn("Todo", 1);
        db.Boards.Add(otherBoard);
        db.Tasks.Add(new TaskItem(otherCol.Id, "T3", "D3", "🔥", 1));

        await db.SaveChangesAsync();

        var repo = new TaskRepository(db);

        var result = await repo.GetByBoardAsync(boardId, userId);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.Name == "T1" || t.Name == "T2");
    }
}
