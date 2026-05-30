using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Persistence.Repositories;

namespace TaskBoard.Infrastructure.Tests.Persistence;

public class UserRepositoryTests
{
    private AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistUser()
    {
        using var db = CreateDb();
        var repo = new UserRepository(db);

        var user = new User("user@example.com", "hash", "Test User");

        await repo.AddAsync(user);

        db.Users.Should().Contain(user);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenExists()
    {
        using var db = CreateDb();
        var repo = new UserRepository(db);

        var user = new User("user@example.com", "hash", "Test User");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var result = await repo.GetByEmailAsync("user@example.com");

        result.Should().NotBeNull();
        result!.Email.Should().Be("user@example.com");
    }
}
