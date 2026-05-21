using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Persistence.Repositories;

namespace TaskBoard.Infrastructure.Tests.Persistence;

public class BoardRepositoryTests
{
    private AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistBoard()
    {
        using var db = CreateDb();
        var repo = new BoardRepository(db);

        var board = new Board(Guid.NewGuid(), "Board");

        await repo.AddAsync(board);

        db.Boards.Should().Contain(board);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBoard_WhenExists()
    {
        using var db = CreateDb();
        var repo = new BoardRepository(db);

        var userId = Guid.NewGuid();
        var board = new Board(userId, "Board");

        db.Boards.Add(board);
        await db.SaveChangesAsync();

        var result = await repo.GetByIdAsync(board.Id, userId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(board.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyBoard()
    {
        using var db = CreateDb();
        var repo = new BoardRepository(db);

        var board = new Board(Guid.NewGuid(), "Old");
        db.Boards.Add(board);
        await db.SaveChangesAsync();

        board.Update("New", null);

        await repo.UpdateAsync(board);

        db.Boards.First().Name.Should().Be("New");
    }
}
