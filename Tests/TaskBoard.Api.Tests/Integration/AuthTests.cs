using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
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

        var emailSender = _factory.Services.GetRequiredService<IEmailSender>() as FakeEmailSender;
        emailSender?.Reset();
    }

    private async Task<(string AccessToken, string RefreshToken)> RegisterAndLoginAsync()
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
    public async Task ForgotPassword_ShouldAlwaysReturnOk()
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

        var emailSender = _factory.Services.GetRequiredService<IEmailSender>() as FakeEmailSender;
        emailSender!.Sent.Should().ContainSingle();
        emailSender.Sent[0].To.Should().Be("test@example.com");
        emailSender.Sent[0].Body.Should().Contain(token.Token);
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

        await db.Entry(user).ReloadAsync();
        await db.Entry(token).ReloadAsync();
        await db.Entry(refresh).ReloadAsync();

        user.PasswordHash.Should().NotBe("oldhash");
        token.Used.Should().BeTrue();
        refresh.Revoked.Should().BeTrue();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
