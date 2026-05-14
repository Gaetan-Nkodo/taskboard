using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskBoard.Api.Tests.integration.Setup;
using TaskBoard.Api.Tests.Setup;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using Xunit;

namespace TaskBoard.Api.Tests.Tasks;

public class TasksTests : IClassFixture<SqlServerContainerFixture>
{
    private readonly HttpClient _client;

    public TasksTests(SqlServerContainerFixture fixture)
    {
        var factory = new ApiFactory(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var register = new RegisterUserRequest("tasks@mail.com", "Password123!");
        await _client.PostAsJsonAsync("/auth/register", register);

        var login = new LoginUserRequest("tasks@mail.com", "Password123!");
        var response = await _client.PostAsJsonAsync("/auth/login", login);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>()
                   ?? throw new Exception("Login response null");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", body["token"]);
    }

    private async Task<(Guid boardId, Guid columnId)> CreateBoardAsync()
    {
        var request = new CreateBoardRequest("Board for tasks", "desc");
        var response = await _client.PostAsJsonAsync("/boards", request);

        var created = await response.Content.ReadFromJsonAsync<BoardDto>()
                      ?? throw new Exception("Board creation null");

        var boardResponse = await _client.GetAsync($"/boards/{created.Id}");
        var board = await boardResponse.Content.ReadFromJsonAsync<BoardDto>()
                    ?? throw new Exception("Board response null");

        var firstColumn = board.Columns.FirstOrDefault()
                          ?? throw new Exception("No column found");

        return (created.Id, firstColumn.Id);
    }

    [Fact]
    public async Task Should_Create_Task()
    {
        await AuthenticateAsync();
        var (boardId, columnId) = await CreateBoardAsync();

        var request = new CreateTaskRequest(columnId, "Task A", "Desc", "📌");

        var response = await _client.PostAsJsonAsync($"/boards/{boardId}/tasks", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<TaskDto>()
                      ?? throw new Exception("Task creation null");

        Assert.NotEqual(Guid.Empty, created.Id);
    }

    [Fact]
    public async Task Should_Update_Task()
    {
        await AuthenticateAsync();
        var (boardId, columnId) = await CreateBoardAsync();

        var create = new CreateTaskRequest(columnId, "Old", "Old Desc", "📌");
        var createResponse = await _client.PostAsJsonAsync($"/boards/{boardId}/tasks", create);

        var created = await createResponse.Content.ReadFromJsonAsync<TaskDto>()
                      ?? throw new Exception("Task creation null");

        var update = new UpdateTaskRequest(columnId, "New", "New Desc", "⭐");
        var response = await _client.PutAsJsonAsync($"/tasks/{created.Id}", update);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var boardResponse = await _client.GetAsync($"/boards/{boardId}");
        var board = await boardResponse.Content.ReadFromJsonAsync<BoardDto>()
                    ?? throw new Exception("Board response null");

        Assert.Contains(board.Columns.SelectMany(c => c.Tasks),
            t => t.Name == "New");
    }

    [Fact]
    public async Task Should_Delete_Task()
    {
        await AuthenticateAsync();
        var (boardId, columnId) = await CreateBoardAsync();

        var create = new CreateTaskRequest(columnId, "To Delete", null, "📌");
        var createResponse = await _client.PostAsJsonAsync($"/boards/{boardId}/tasks", create);

        var created = await createResponse.Content.ReadFromJsonAsync<TaskDto>()
                      ?? throw new Exception("Task creation null");

        var response = await _client.DeleteAsync($"/tasks/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var boardResponse = await _client.GetAsync($"/boards/{boardId}");
        var board = await boardResponse.Content.ReadFromJsonAsync<BoardDto>()
                    ?? throw new Exception("Board response null");

        Assert.DoesNotContain(board.Columns.SelectMany(c => c.Tasks),
            t => t.Id == created.Id);
    }
}
