using System.Net;
using System.Net.Http.Json;
using TaskBoard.Api.Tests.Integration.Setup;
using TaskBoard.Application.Requests;

namespace TaskBoard.Api.Tests.Integration.Auth;

public class AuthTests : IClassFixture<SqlServerContainerFixture>
{
    private readonly HttpClient _client;

    public AuthTests(SqlServerContainerFixture fixture)
    {
        var factory = new ApiFactory(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_Register_User()
    {
        var request = new RegisterUserRequest("test@mail.com", "Password123!");

        var response = await _client.PostAsJsonAsync("/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(body);
        Assert.True(body!.ContainsKey("id"));
    }

    [Fact]
    public async Task Should_Login_User()
    {
        // Register
        var register = new RegisterUserRequest("login@mail.com", "Password123!");
        await _client.PostAsJsonAsync("/auth/register", register);

        // Login
        var login = new LoginUserRequest("login@mail.com", "Password123!");
        var response = await _client.PostAsJsonAsync("/auth/login", login);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(body);
        Assert.True(body!.ContainsKey("token"));
        Assert.False(string.IsNullOrWhiteSpace(body["token"]));
    }

    [Fact]
    public async Task Should_Return_401_When_Invalid_Credentials()
    {
        var login = new LoginUserRequest("unknown@mail.com", "wrong");

        var response = await _client.PostAsJsonAsync("/auth/login", login);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Should_Return_400_When_Email_Already_Used()
    {
        var request = new RegisterUserRequest("duplicate@mail.com", "Password123!");

        await _client.PostAsJsonAsync("/auth/register", request);
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
