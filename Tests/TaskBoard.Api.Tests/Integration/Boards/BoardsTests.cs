
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskBoard.Api.Tests.Integration.Setup;
using TaskBoard.Application.Requests;

namespace TaskBoard.Api.Tests.Integration.Boards;

public class BoardsTests : IClassFixture<SqlServerContainerFixture>
{
    private readonly HttpClient _client;

    public BoardsTests(SqlServerContainerFixture fixture)
    {
        var factory = new ApiFactory(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var register = new RegisterUserRequest("boards@mail.com", "Password123!");
        await _client.PostAsJsonAsync("/auth/register", register);

        var login = new LoginUserRequest("boards@mail.com", "Password123!");
        var response = await _client.PostAsJsonAsync("/auth/login", login);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var token = body!["token"];

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task Should_Create_Board()
    {
        await AuthenticateAsync();

        var request = new CreateBoardRequest("My Board", "Description");

        var response = await _client.PostAsJsonAsync("/boards", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(body);
        Assert.True(body!.ContainsKey("id"));
    }

    [Fact]
    public async Task Should_Get_Board()
    {
        await AuthenticateAsync();

        // Create
        var create = new CreateBoardRequest("Board A", "Desc");
        var createResponse = await _client.PostAsJsonAsync("/boards", create);
        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var boardId = Guid.Parse(created!["id"].ToString()!);

        // Get
        var response = await _client.GetAsync($"/boards/{boardId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var board = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.Equal(boardId.ToString(), board!["id"].ToString());
    }

    [Fact]
    public async Task Should_Get_All_Boards()
    {
        await AuthenticateAsync();

        await _client.PostAsJsonAsync("/boards", new CreateBoardRequest("Board 1", null));
        await _client.PostAsJsonAsync("/boards", new CreateBoardRequest("Board 2", null));

        var response = await _client.GetAsync("/boards");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var boards = await response.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
        Assert.True(boards!.Count >= 2);
    }

    [Fact]
    public async Task Should_Update_Board()
    {
        await AuthenticateAsync();

        // Create
        var create = new CreateBoardRequest("Old Name", "Old Desc");
        var createResponse = await _client.PostAsJsonAsync("/boards", create);
        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var boardId = Guid.Parse(created!["id"].ToString()!);

        // Update
        var update = new UpdateBoardRequest("New Name", "New Desc");
        var response = await _client.PutAsJsonAsync($"/boards/{boardId}", update);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify
        var get = await _client.GetAsync($"/boards/{boardId}");
        var board = await get.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        Assert.Equal("New Name", board!["name"].ToString());
    }

    [Fact]
    public async Task Should_Delete_Board()
    {
        await AuthenticateAsync();

        // Create
        var create = new CreateBoardRequest("To Delete", null);
        var createResponse = await _client.PostAsJsonAsync("/boards", create);
        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var boardId = Guid.Parse(created!["id"].ToString()!);

        // Delete
        var response = await _client.DeleteAsync($"/boards/{boardId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify
        var get = await _client.GetAsync($"/boards/{boardId}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }
}
