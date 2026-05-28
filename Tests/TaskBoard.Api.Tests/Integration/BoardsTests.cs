using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using TaskBoard.Api.Tests.Fixtures;
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

    private async Task<string> AuthenticateAsync()
    {
        var email = $"user{Guid.NewGuid()}@example.com";

        var register = new RegisterUserRequest(email, "P@ssw0rd!");
        var regResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", register);
        regResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = new LoginUserRequest(email, "P@ssw0rd!");
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", login);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        result.Should().NotBeNull();

        return result!.Token;
    }

    [Fact]
    public async Task CreateBoard_ShouldReturnCreated()
    {
        var token = await AuthenticateAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateBoardRequest("My Board", "Desc");
        var response = await _client.PostAsJsonAsync("/api/v1/boards", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetBoards_ShouldReturnList()
    {
        var token = await AuthenticateAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/v1/boards");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
