using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using TaskBoard.Api.Tests.Fixtures;
using TaskBoard.Api.Tests.Utils;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;

using static TaskBoard.Api.Tests.Integration.TasksTests;

namespace TaskBoard.Api.Tests.Integration;

[Collection("Api collection")]
public class ColumnsTests
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public ColumnsTests(ApiFactory factory, PostgresContainerFixture fixture)
    {
        _factory = factory;
        factory.SetConnectionString(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var email = $"user{Guid.NewGuid()}@example.com";

        // 1. REGISTER
        var register = new RegisterUserRequest(email, "P@ssw0rd!", "Test User");
        var regResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", register);
        regResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 2. RÉCUPÉRER LE TOKEN DE CONFIRMATION EN DB
        using (var scope = new ApiScope(_factory))
        {
            var db = scope.Db;

            var user = await db.Users.FirstAsync(u => u.Email == email);
            var emailToken = await db.EmailVerificationTokens.FirstAsync(t => t.UserId == user.Id);

            // 3. CONFIRMER L’EMAIL
            var confirmResponse = await _client.GetAsync($"/api/v1/auth/confirm-email?token={emailToken.Token}");
            confirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // 4. LOGIN
        var login = new LoginUserRequest(email, "P@ssw0rd!");
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", login);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();
        result.Should().NotBeNull();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result!.AccessToken);
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
