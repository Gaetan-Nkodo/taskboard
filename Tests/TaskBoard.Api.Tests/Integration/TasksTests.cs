using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using TaskBoard.Api.Tests.Fixtures;
using TaskBoard.Application.Requests;

namespace TaskBoard.Api.Tests.Integration;

[Collection("Api collection")]
public class TasksTests
{
    private readonly HttpClient _client;

    public TasksTests(ApiFactory factory, SqlServerContainerFixture fixture)
    {
        factory.SetConnectionString(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var email = $"user{Guid.NewGuid()}@example.com";

        await _client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterUserRequest(email, "P@ssw0rd!"));

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginUserRequest(email, "P@ssw0rd!"));

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", login!.Token);
    }

    [Fact]
    public async Task CreateTask_ShouldAddTaskToColumn()
    {
        await AuthenticateAsync();

        var boardResponse = await _client.PostAsJsonAsync(
            "/api/v1/boards",
            new CreateBoardRequest("Board", "Desc")
        );

        boardResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var boardJson = await boardResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var boardId = boardJson!["id"];

        var getBoardResponse = await _client.GetAsync($"/api/v1/boards/{boardId}");
        var board = await getBoardResponse.Content.ReadFromJsonAsync<BoardResponseDto>();

        var columnId = board!.Columns[0].Id;

        var request = new CreateTaskRequest(columnId, "Task", "Desc", "🔥");

        var response = await _client.PostAsJsonAsync(
            $"/api/v1/boards/{boardId}/tasks",
            request
        );

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
