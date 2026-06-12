using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using TaskBoard.Api.Tests.Fixtures;
using TaskBoard.Api.Tests.Utils;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Api.Tests.Integration;

[Collection("Api collection")]
public class AuthTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public AuthTests(ApiFactory factory, PostgresContainerFixture fixture)
    {
        _factory = factory;
        factory.SetConnectionString(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
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

    [Fact]
    public async Task ForgotPassword_ShouldAlwaysReturnOk()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", new
        {
            email = "unknown@example.com"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ForgotPassword_ShouldReturnOk_EvenIfEmailDoesNotExist()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", new
        {
            email = "unknown@example.com"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ForgotPassword_ShouldCreateToken_AndSendEmail()
    {
        using var scope = new ApiScope(_factory);
        var db = scope.Db;

        var user = new User("test@example.com", "hash", "Test User");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", new
        {
            email = "test@example.com"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var token = db.PasswordResetTokens.FirstOrDefault(t => t.UserId == user.Id);
        token.Should().NotBeNull();
        token!.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

        var emailService = _factory.Services.GetRequiredService<IEmailService>() as FakeEmailService;
        emailService!.Sent.Should().ContainSingle();
        emailService.Sent[0].Email.Should().Be("test@example.com");
        emailService.Sent[0].Link.Should().Contain(token.Token);
    }

    [Fact]
    public async Task ResetPassword_ShouldFail_WhenTokenDoesNotExist()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/reset-password", new
        {
            token = "UNKNOWN",
            newPassword = "P@ssw0rd!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ResetPassword_ShouldFail_WhenTokenExpired()
    {
        using var scope = new ApiScope(_factory);
        var db = scope.Db;

        var user = new User("expired@example.com", "hash", "Expired User");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var token = new PasswordResetToken(
            user.Id,
            "EXPIRED",
            DateTime.UtcNow.AddMinutes(-10)
        );

        db.PasswordResetTokens.Add(token);
        await db.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync("/api/v1/auth/reset-password", new
        {
            token = "EXPIRED",
            newPassword = "NewP@ssw0rd!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ResetPassword_ShouldFail_WhenTokenAlreadyUsed()
    {
        using var scope = new ApiScope(_factory);
        var db = scope.Db;

        var user = new User("used@example.com", "hash", "Used User");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var token = new PasswordResetToken(
            user.Id,
            "USED",
            DateTime.UtcNow.AddMinutes(10)
        );
        token.MarkUsed();

        db.PasswordResetTokens.Add(token);
        await db.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync("/api/v1/auth/reset-password", new
        {
            token = "USED",
            newPassword = "NewP@ssw0rd!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ResetPassword_ShouldUpdatePassword_AndRevokeRefreshTokens()
    {
        using var scope = new ApiScope(_factory);
        var db = scope.Db;

        var user = new User("valid@example.com", "oldhash", "Valid User");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var token = new PasswordResetToken(
            user.Id,
            "VALID",
            DateTime.UtcNow.AddMinutes(10)
        );

        db.PasswordResetTokens.Add(token);

        var refresh = new RefreshToken(user.Id, "REFRESH1", DateTime.UtcNow.AddDays(1));
        db.RefreshTokens.Add(refresh);

        await db.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync("/api/v1/auth/reset-password", new
        {
            token = "VALID",
            newPassword = "NewP@ssw0rd!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // 🔥 Forcer le rechargement depuis la DB
        await db.Entry(user).ReloadAsync();
        await db.Entry(token).ReloadAsync();
        await db.Entry(refresh).ReloadAsync();

        var updated = user;
        updated!.PasswordHash.Should().NotBe("oldhash");

        var updatedToken = token;
        updatedToken!.Used.Should().BeTrue();

        var revoked = refresh;
        revoked!.Revoked.Should().BeTrue();
    }

    [Fact]
    public async Task ChangePassword_ShouldUpdatePassword_AndRevokeRefreshTokens()
    {
        using var scope = new ApiScope(_factory);
        var db = scope.Db;

        // Arrange : créer un user
        var email = $"user{Guid.NewGuid()}@example.com";
        var register = new RegisterUserRequest(email, "OLD", "Test User");
        await _client.PostAsJsonAsync("/api/v1/auth/register", register);

        // Login
        var login = new LoginUserRequest(email, "OLD");
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", login);
        var tokens = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();

        // Ajouter un refresh token en DB
        var user = db.Users.First(u => u.Email == email);
        var refresh = new RefreshToken(user.Id, "REFRESH1", DateTime.UtcNow.AddDays(1));
        db.RefreshTokens.Add(refresh);
        await db.SaveChangesAsync();

        // Authentifier le client
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/change-password", new
        {
            currentPassword = "OLD",
            newPassword = "NEW"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // 🔥 Recharger les entités depuis la DB
        await db.Entry(user).ReloadAsync();
        await db.Entry(refresh).ReloadAsync();

        var updated = user;
        updated!.PasswordHash.Should().Be("NEW");

        var revoked = refresh;
        revoked!.Revoked.Should().BeTrue();
    }

    [Fact]
    public async Task ChangePassword_ShouldFail_WhenCurrentPasswordIncorrect()
    {
        using var scope = new ApiScope(_factory);
        var db = scope.Db;

        var email = $"user{Guid.NewGuid()}@example.com";
        await _client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterUserRequest(email, "OLD", "Test User"));

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginUserRequest(email, "OLD"));

        var tokens = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/change-password", new
        {
            currentPassword = "BAD",
            newPassword = "NEW"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_ShouldFail_WhenUserNotAuthenticated()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/change-password", new
        {
            currentPassword = "ANY",
            newPassword = "NEW"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
