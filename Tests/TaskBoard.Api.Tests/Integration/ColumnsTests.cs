using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using TaskBoard.Api.Tests.Fixtures;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;

using static TaskBoard.Api.Tests.Integration.TasksTests;

namespace TaskBoard.Api.Tests.Integration;

[Collection("Api collection")]
public class ColumnsTests
{
    private readonly HttpClient _client;

    public ColumnsTests(ApiFactory factory, PostgresContainerFixture fixture)
    {
        factory.SetConnectionString(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var email = $"user{Guid.NewGuid()}@example.com";

        await _client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterUserRequest(email, "P@ssw0rd!", "Test User"));

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginUserRequest(email, "P@ssw0rd!"));

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", login!.AccessToken);
    }

    [Fact]
    public async Task CreateBoard_ShouldContainDefaultColumns()
    {
        await AuthenticateAsync();

        var response = await _client.PostAsJsonAsync("/api/v1/boards",
            new CreateBoardRequest("Board", "Desc"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var board = await response.Content.ReadFromJsonAsync<IdResponse>();
        board.Should().NotBeNull();

        var boardDetails = await _client.GetFromJsonAsync<BoardDto>($"/api/v1/boards/{board.Id}");
        boardDetails.Should().NotBeNull();

        boardDetails!.Columns.Should().HaveCount(5);

        boardDetails.Columns.Select(c => c.Name).Should().Contain(new[]
        {
            "Backlog",
            "Ready",
            "In Progress",
            "Review",
            "Done"
        });
    }
}
