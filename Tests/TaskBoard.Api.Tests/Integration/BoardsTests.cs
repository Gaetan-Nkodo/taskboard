using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using TaskBoard.Api.Tests.Fixtures;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;

namespace TaskBoard.Api.Tests.Integration;

[Collection("Api collection")]
public class BoardsTests
{
    private readonly HttpClient _client;

    public BoardsTests(ApiFactory factory, PostgresContainerFixture fixture)
    {
        factory.SetConnectionString(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var email = $"user{Guid.NewGuid()}@example.com";

        var register = new RegisterUserRequest(email, "P@ssw0rd!", "Test User");
        var regResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", register);
        regResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = new LoginUserRequest(email, "P@ssw0rd!");
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", login);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();
        result.Should().NotBeNull();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.AccessToken);
    }

    [Fact]
    public async Task CreateBoard_ShouldReturnCreated()
    {
        await AuthenticateAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/boards")
        {
            Content = JsonContent.Create(new CreateBoardRequest("My Board", "Desc"))
        };

        request.Headers.Authorization = _client.DefaultRequestHeaders.Authorization;

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetBoards_ShouldReturnList()
    {
        await AuthenticateAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/boards");
        request.Headers.Authorization = _client.DefaultRequestHeaders.Authorization;

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
