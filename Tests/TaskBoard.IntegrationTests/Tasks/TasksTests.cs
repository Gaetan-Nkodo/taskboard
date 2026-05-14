using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskBoard.Api.Tests.Integration.Setup;
using TaskBoard.Application.Requests;
using Xunit;

namespace TaskBoard.Api.Tests.Integration.Tasks;

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
                   ?? throw new Exception("Login response is null");

        var token = body["token"];

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<(Guid boardId, Guid columnId)> CreateBoardAsync()
    {
        var request = new CreateBoardRequest("Board for tasks", "desc");
        var response = await _client.PostAsJsonAsync("/boards", request);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>()
                   ?? throw new Exception("Board creation response is null");

        var boardId = Guid.Parse(body["id"].ToString()!);

        var boardResponse = await _client.GetAsync($"/boards/{boardId}");
        var board = await boardResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>()
                    ?? throw new Exception("Board response is null");

        var columns = board["columns"] as IEnumerable<object>
                      ?? throw new Exception("Columns missing");

        var firstColumn = columns.FirstOrDefault() as Dictionary<string, object>
                          ?? throw new Exception("No column found");

        var columnId = Guid.Parse(firstColumn["id"].ToString()!);

        return (boardId, columnId);
    }
}
