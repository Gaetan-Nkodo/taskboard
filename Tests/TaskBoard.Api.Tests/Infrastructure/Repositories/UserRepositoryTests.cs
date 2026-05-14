using TaskBoard.Api.Tests.integration.Setup;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Persistence.Repositories;

namespace TaskBoard.Api.Tests.Infrastructure.Repositories;

public class UserRepositoryTests : IClassFixture<SqlServerContainerFixture>
{
    private readonly SqlServerContainerFixture _fixture;

    public UserRepositoryTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_Add_And_Get_User_By_Id()
    {
        // Arrange
        var repo = new UserRepository(_fixture.Db);

        var user = new User("test@mail.com", "hashed-password");

        // Act
        await repo.AddAsync(user);
        var loaded = await repo.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal("test@mail.com", loaded!.Email);
        Assert.Equal("hashed-password", loaded.PasswordHash);
    }

    [Fact]
    public async Task Should_Add_And_Get_User_By_Email()
    {
        // Arrange
        var repo = new UserRepository(_fixture.Db);

        var user = new User("user@mail.com", "hash123");
        await repo.AddAsync(user);

        // Act
        var loaded = await repo.GetByEmailAsync("user@mail.com");

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal(user.Id, loaded!.Id);
        Assert.Equal("hash123", loaded.PasswordHash);
    }

    [Fact]
    public async Task Should_Return_Null_When_Email_Not_Found()
    {
        // Arrange
        var repo = new UserRepository(_fixture.Db);

        // Act
        var loaded = await repo.GetByEmailAsync("unknown@mail.com");

        // Assert
        Assert.Null(loaded);
    }

    [Fact]
    public async Task Should_Not_Track_Entities_On_Read()
    {
        // Arrange
        var repo = new UserRepository(_fixture.Db);

        var user = new User("track@mail.com", "hash");
        await repo.AddAsync(user);

        // Act
        var loaded = await repo.GetByEmailAsync("track@mail.com");

        // Assert
        Assert.NotNull(loaded);

        // Vérifie que l'entité n'est pas suivie par EF Core
        var entry = _fixture.Db.ChangeTracker.Entries<User>().FirstOrDefault();
        Assert.Null(entry);
    }

    [Fact]
    public async Task Should_Handle_Multiple_Users_Correctly()
    {
        // Arrange
        var repo = new UserRepository(_fixture.Db);

        var u1 = new User("a@mail.com", "h1");
        var u2 = new User("b@mail.com", "h2");
        var u3 = new User("c@mail.com", "h3");

        await repo.AddAsync(u1);
        await repo.AddAsync(u2);
        await repo.AddAsync(u3);

        // Act
        var loaded = await repo.GetByEmailAsync("b@mail.com");

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal("b@mail.com", loaded!.Email);
        Assert.Equal("h2", loaded.PasswordHash);
    }
}
