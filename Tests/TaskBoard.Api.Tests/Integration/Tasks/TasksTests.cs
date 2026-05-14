using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskBoard.Api.Tests.Integration.Setup;
using TaskBoard.Application.Requests;

namespace TaskBoard.Api.Tests.Integration.Tasks;

public class TasksTests : IClassFixture<SqlServerContainerFixture>
{
    private readonly HttpClient _client;

    public TasksTests(SqlServerContainerFixture fixture)
    {
        var factory = new ApiFactory(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var register = new RegisterUserRequest("tasks@mail.com", "Password123!");
        await _client.PostAsJsonAsync("/auth/register", register);

        var login = new LoginUserRequest("tasks@mail.com", "Password123!");
        var response = await _client.PostAsJsonAsync("/auth/login", login);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var token = body!["token"];

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<(Guid boardId, Guid columnId)> CreateBoardAsync()
    {
        var request = new CreateBoardRequest("Board for tasks", "desc");
        var response = await _client.PostAsJsonAsync("/boards", request);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var boardId = Guid.Parse(body!["id"].ToString()!);

        // Load board to get default columns
        var boardResponse = await _client.GetAsync($"/boards/{boardId}");
        var board = await boardResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        var columns = board!["columns"] as IEnumerable<object>;
        var firstColumn = columns!.First() as Dictionary<string, object>;
        var columnId = Guid.Parse(firstColumn!["id"].ToString()!);

        return (boardId, columnId);
    }

    [Fact]
    public async Task Should_Create_Task()
    {
        await AuthenticateAsync();
        var (boardId, columnId) = await CreateBoardAsync();

        var request = new CreateTaskRequest(columnId, "Task A", "Desc", "📌");

        var response = await _client.PostAsJsonAsync($"/boards/{boardId}/tasks", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.True(body!.ContainsKey("id"));
    }

    [Fact]
    public async Task Should_Update_Task()
    {
        await AuthenticateAsync();
        var (boardId, columnId) = await CreateBoardAsync();

        // Create task
        var create = new CreateTaskRequest(columnId, "Old", "Old Desc", "📌");
        var createResponse = await _client.PostAsJsonAsync($"/boards/{boardId}/tasks", create);
        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var taskId = Guid.Parse(created!["id"].ToString()!);

        // Update
        var update = new UpdateTaskRequest(columnId, "New", "New Desc", "⭐");
        var response = await _client.PutAsJsonAsync($"/tasks/{taskId}", update);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify via board
        var boardResponse = await _client.GetAsync($"/boards/{boardId}");
        var board = await boardResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        var columns = board!["columns"] as IEnumerable<object>;
        var tasks = columns!.SelectMany(c =>
            ((Dictionary<string, object>)c)["tasks"] as IEnumerable<object>
        );

        Assert.Contains(tasks!, t => t.ToString()!.Contains("New"));
    }

    [Fact]
    public async Task Should_Delete_Task()
    {
        await AuthenticateAsync();
        var (boardId, columnId) = await CreateBoardAsync();

        // Create task
        var create = new CreateTaskRequest(columnId, "To Delete", null, "📌");
        var createResponse = await _client.PostAsJsonAsync($"/boards/{boardId}/tasks", create);
        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var taskId = Guid.Parse(created!["id"].ToString()!);

        // Delete
        var response = await _client.DeleteAsync($"/tasks/{taskId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify via board
        var boardResponse = await _client.GetAsync($"/boards/{boardId}");
        var board = await boardResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        var columns = board!["columns"] as IEnumerable<object>;
        var tasks = columns!.SelectMany(c =>
            ((Dictionary<string, object>)c)["tasks"] as IEnumerable<object>
        );

        Assert.DoesNotContain(tasks!, t => t.ToString()!.Contains(taskId.ToString()));
    }

    [Fact]
    public async Task Should_Return_404_When_Task_Not_Found()
    {
        await AuthenticateAsync();

        var update = new UpdateTaskRequest(Guid.NewGuid(), "X", "Y", "📌");

        var response = await _client.PutAsJsonAsync($"/tasks/{Guid.NewGuid()}", update);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
