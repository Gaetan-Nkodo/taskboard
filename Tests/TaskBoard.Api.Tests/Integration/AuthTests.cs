using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using TaskBoard.Api.Tests.Fixtures;
using TaskBoard.Api.Tests.Utils;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Api.Tests.Integration;

[Collection("Api collection")]
public class AuthTests
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public AuthTests(ApiFactory factory, PostgresContainerFixture fixture)
    {
        _factory = factory;
        factory.SetConnectionString(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    private async Task<(string AccessToken, string RefreshToken)> RegisterAndLoginAsync()
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

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var logoutResponse = await _client.PostAsync("/api/v1/auth/logout", null);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh", new
        {
            refreshToken
        });

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenTokenDoesNotExist()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", new
        {
            refreshToken = "DOES_NOT_EXIST"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenTokenIsExpired()
    {
        using var scope = new ApiScope(_factory);
        var db = scope.Db;

        var user = new User("expired@example.com", "hash", "Expired User");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var expired = new RefreshToken(user.Id, "EXPIRED_TOKEN", DateTime.UtcNow.AddMinutes(-10));
        db.RefreshTokens.Add(expired);
        await db.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", new
        {
            refreshToken = "EXPIRED_TOKEN"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenTokenIsRevoked()
    {
        using var scope = new ApiScope(_factory);
        var db = scope.Db;

        var user = new User("revoked@example.com", "hash", "Revoked User");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var token = new RefreshToken(user.Id, "REVOKED_TOKEN", DateTime.UtcNow.AddMinutes(10));
        token.Revoke();
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", new
        {
            refreshToken = "REVOKED_TOKEN"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        using var scope = new ApiScope(_factory);
        var db = scope.Db;

        var ghostUserId = Guid.NewGuid();

        var token = new RefreshToken(ghostUserId, "GHOST_TOKEN", DateTime.UtcNow.AddMinutes(10));
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", new
        {
            refreshToken = "GHOST_TOKEN"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenUserIsInactive()
    {
        using var scope = new ApiScope(_factory);
        var db = scope.Db;

        var user = new User("inactive@example.com", "hash", "Inactive User");
        user.Deactivate();
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var token = new RefreshToken(user.Id, "INACTIVE_TOKEN", DateTime.UtcNow.AddMinutes(10));
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", new
        {
            refreshToken = "INACTIVE_TOKEN"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
