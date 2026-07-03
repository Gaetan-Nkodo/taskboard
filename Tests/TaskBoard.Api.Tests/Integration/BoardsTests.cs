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
public class BoardsTests
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public BoardsTests(ApiFactory factory, PostgresContainerFixture fixture)
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
