using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskBoard.Api.Tests.Fixtures;
using TaskBoard.Application.Requests;

namespace TaskBoard.Api.Tests.Integration;

[Collection("Api collection")]
public class AuthTests
{
    private readonly HttpClient _client;

    public AuthTests(ApiFactory factory, SqlServerContainerFixture fixture)
    {
        factory.SetConnectionString(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Then_Login_ShouldReturnJwt()
    {
        var email = $"user{Guid.NewGuid()}@example.com";

        var register = new RegisterUserRequest(email, "P@ssw0rd!");
        var login = new LoginUserRequest(email, "P@ssw0rd!");

        var regResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", register);
        regResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", login);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrWhiteSpace();
        result.User.Email.Should().Be(email);
    }
}
