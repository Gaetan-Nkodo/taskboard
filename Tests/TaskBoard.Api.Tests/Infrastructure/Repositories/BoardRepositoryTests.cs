using TaskBoard.Api.Tests.integration.Setup;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Persistence.Repositories;

namespace TaskBoard.Api.Tests.Infrastructure.Repositories;

public class BoardRepositoryTests : IClassFixture<SqlServerContainerFixture>
{
    private readonly SqlServerContainerFixture _fixture;

    public BoardRepositoryTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_Add_And_Get_Board()
    {
        // Arrange
        var repo = new BoardRepository(_fixture.Db);

        var userId = Guid.NewGuid();
        var board = new Board(userId, "My Board", "Description");

        // Act
        await repo.AddAsync(board);
        var loaded = await repo.GetByIdAsync(board.Id, userId);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal("My Board", loaded!.Name);
        Assert.Equal("Description", loaded.Description);
    }

    [Fact]
    public async Task Should_Add_Board_With_Columns_And_Tasks_And_Load_Them()
    {
        // Arrange
        var repo = new BoardRepository(_fixture.Db);

        var userId = Guid.NewGuid();
        var board = new Board(userId, "Board");

        var col1 = board.AddColumn("In Progress", 1);
        var col2 = board.AddColumn("Completed", 2);

        col1.AddTask("Task 1", "Desc 1", "📌");
        col2.AddTask("Task 2", "Desc 2", "⭐");

        // Act
        await repo.AddAsync(board);
        var loaded = await repo.GetByIdAsync(board.Id, userId);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal(2, loaded!.Columns.Count);

        var loadedCol1 = loaded.Columns.First(c => c.Name == "In Progress");
        var loadedCol2 = loaded.Columns.First(c => c.Name == "Completed");

        Assert.Single(loadedCol1.Tasks);
        Assert.Single(loadedCol2.Tasks);

        Assert.Contains(loadedCol1.Tasks, t => t.Name == "Task 1");
        Assert.Contains(loadedCol2.Tasks, t => t.Name == "Task 2");
    }

    [Fact]
    public async Task Should_Get_Boards_By_UserId()
    {
        // Arrange
        var repo = new BoardRepository(_fixture.Db);

        var userId = Guid.NewGuid();

        var board1 = new Board(userId, "Board 1");
        var board2 = new Board(userId, "Board 2");

        await repo.AddAsync(board1);
        await repo.AddAsync(board2);

        // Act
        var boards = await repo.GetByUserIdAsync(userId);

        // Assert
        Assert.Equal(2, boards.Count);
        Assert.Contains(boards, b => b.Name == "Board 1");
        Assert.Contains(boards, b => b.Name == "Board 2");
    }

    [Fact]
    public async Task Should_Update_Board()
    {
        // Arrange
        var repo = new BoardRepository(_fixture.Db);

        var userId = Guid.NewGuid();
        var board = new Board(userId, "Old", "Old Desc");
        await repo.AddAsync(board);

        // Act
        board.Update("New", "New Desc");
        await repo.UpdateAsync(board);

        var loaded = await repo.GetByIdAsync(board.Id, userId);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal("New", loaded!.Name);
        Assert.Equal("New Desc", loaded.Description);
    }

    [Fact]
    public async Task Should_Delete_Board()
    {
        // Arrange
        var repo = new BoardRepository(_fixture.Db);

        var userId = Guid.NewGuid();
        var board = new Board(userId, "Board");
        await repo.AddAsync(board);

        // Act
        await repo.DeleteAsync(board);
        var loaded = await repo.GetByIdAsync(board.Id, userId);

        // Assert
        Assert.Null(loaded);
    }
}
