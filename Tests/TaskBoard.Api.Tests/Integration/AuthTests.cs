using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using TaskBoard.Api.Tests.Fixtures;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;

namespace TaskBoard.Api.Tests.Integration;

[Collection("Api collection")]
public class AuthTests
{
    private readonly HttpClient _client;

    public AuthTests(ApiFactory factory, PostgresContainerFixture fixture)
    {
        factory.SetConnectionString(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    private async Task<(string AccessToken, string RefreshToken)> RegisterAndLoginAsync()
    {
        var email = $"user{Guid.NewGuid()}@example.com";

        // REGISTER
        var register = new RegisterUserRequest(email, "P@ssw0rd!", "Test User");
        var regResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", register);
        regResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // LOGIN
        var login = new LoginUserRequest(email, "P@ssw0rd!");
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", login);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();
        result.Should().NotBeNull();

        return (result!.AccessToken, result.RefreshToken);
    }

    [Fact]
    public async Task Register_ShouldReturnOk()
    {
        var email = $"user{Guid.NewGuid()}@example.com";

        var request = new RegisterUserRequest(email, "P@ssw0rd!", "Test User");

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.Email.Should().Be(email);
    }

    [Fact]
    public async Task Login_ShouldReturnTokens()
    {
        var (access, refresh) = await RegisterAndLoginAsync();

        access.Should().NotBeNullOrWhiteSpace();
        refresh.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Refresh_ShouldReturnNewTokens()
    {
        var (_, refreshToken) = await RegisterAndLoginAsync();

        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", new
        {
            refreshToken
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<LoginResultDto>();
        result.Should().NotBeNull();

        result!.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Logout_ShouldRevokeTokens_AndPreventRefresh()
    {
        var (accessToken, refreshToken) = await RegisterAndLoginAsync();

        // LOGOUT
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var logoutResponse = await _client.PostAsync("/api/v1/auth/logout", null);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // REFRESH SHOULD FAIL
        var refreshResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh", new
        {
            refreshToken
        });

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
