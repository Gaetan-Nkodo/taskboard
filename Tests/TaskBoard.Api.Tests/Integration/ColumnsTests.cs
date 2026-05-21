using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskBoard.Api.Tests.Fixtures;
using TaskBoard.Application.Requests;

namespace TaskBoard.Api.Tests.Integration;

[Collection("Api collection")]
public class ColumnsTests
{
    private readonly HttpClient _client;

    public ColumnsTests(ApiFactory factory, SqlServerContainerFixture fixture)
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
    public async Task CreateBoard_ShouldContainDefaultColumns()
    {
        await AuthenticateAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/v1/boards",
            new CreateBoardRequest("Board", "Desc")
        );

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var boardId = json!["id"];

        var boardResponse = await _client.GetAsync($"/api/v1/boards/{boardId}");
        boardResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var board = await boardResponse.Content.ReadFromJsonAsync<BoardResponseDto>();

        board.Should().NotBeNull();
        board!.Columns.Should().HaveCount(3);
    }
}
