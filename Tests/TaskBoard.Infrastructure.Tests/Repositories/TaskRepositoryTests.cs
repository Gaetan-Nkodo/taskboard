using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Persistence.Repositories;
using TaskBoard.IntegrationTests.Database;

namespace TaskBoard.IntegrationTests.Repositories;

public class TaskRepositoryTests : IClassFixture<SqlServerContainerFixture>
{
    private readonly SqlServerContainerFixture _fixture;

    public TaskRepositoryTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_Add_And_Get_Task()
    {
        // Arrange
        var repo = new TaskRepository(_fixture.Db);

        var task = new TaskItem(Guid.NewGuid(), "My Task", "Description", "📌", "Todo");

        // Act
        await repo.AddAsync(task);
        var loaded = await repo.GetByIdAsync(task.Id);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal("My Task", loaded!.Name);
        Assert.Equal("📌", loaded.Icon);
    }

    [Fact]
    public async Task Should_Update_Task()
    {
        // Arrange
        var repo = new TaskRepository(_fixture.Db);

        var task = new TaskItem(Guid.NewGuid(), "Old", "Old Desc", "📌", "Todo");
        await repo.AddAsync(task);

        // Act
        task.Update("New", "New Desc", "⭐", "Done");
        await repo.UpdateAsync(task);

        var loaded = await repo.GetByIdAsync(task.Id);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal("New", loaded!.Name);
        Assert.Equal("⭐", loaded.Icon);
        Assert.Equal("Done", loaded.Status);
    }

    [Fact]
    public async Task Should_Delete_Task()
    {
        // Arrange
        var repo = new TaskRepository(_fixture.Db);

        var task = new TaskItem(Guid.NewGuid(), "Task", "Desc", "📌", "Todo");
        await repo.AddAsync(task);

        // Act
        await repo.DeleteAsync(task);
        var loaded = await repo.GetByIdAsync(task.Id);

        // Assert
        Assert.Null(loaded);
    }
}
