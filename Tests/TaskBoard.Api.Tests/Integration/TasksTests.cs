using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using TaskBoard.Api.Tests.Fixtures;
using TaskBoard.Api.Tests.Utils;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;

namespace TaskBoard.Api.Tests.Integration;

[Collection("Api collection")]
public class TasksTests
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public TasksTests(ApiFactory factory, PostgresContainerFixture fixture)
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

    public record IdResponse(Guid Id);

    // ------------------------------------------------------------
    // U1 — GET /boards/{boardId}/tasks
    // ------------------------------------------------------------
    [Fact]
    public async Task GetTasks_ShouldReturnOnlyTasksOfBoardForUser()
    {
        await AuthenticateAsync();

        var boardResponse = await _client.PostAsJsonAsync("/api/v1/boards",
            new CreateBoardRequest("Board A", "Desc"));

        var board = await boardResponse.Content.ReadFromJsonAsync<IdResponse>();

        var boardDetails = await _client.GetFromJsonAsync<BoardDto>($"/api/v1/boards/{board!.Id}");
        var columnId = boardDetails!.Columns.First().Id;

        await _client.PostAsJsonAsync($"/api/v1/boards/{board.Id}/tasks",
            new CreateTaskRequest(columnId, "T1", "D1", "🔥"));

        await _client.PostAsJsonAsync($"/api/v1/boards/{board.Id}/tasks",
            new CreateTaskRequest(columnId, "T2", "D2", "🔥"));

        var boardBResponse = await _client.PostAsJsonAsync("/api/v1/boards",
            new CreateBoardRequest("Board B", "Desc"));

        var boardB = await boardBResponse.Content.ReadFromJsonAsync<IdResponse>();
        var boardBDetails = await _client.GetFromJsonAsync<BoardDto>($"/api/v1/boards/{boardB!.Id}");
        var columnBId = boardBDetails!.Columns.First().Id;

        await _client.PostAsJsonAsync($"/api/v1/boards/{boardB.Id}/tasks",
            new CreateTaskRequest(columnBId, "T3", "D3", "🔥"));

        var response = await _client.GetAsync($"/api/v1/boards/{board.Id}/tasks");
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();

        tasks.Should().HaveCount(2);
        tasks.Select(t => t.Name).Should().Contain(new[] { "T1", "T2" });
    }

    // ------------------------------------------------------------
    // U2 — POST /boards/{boardId}/tasks
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateTask_ShouldAddTaskToBoard()
    {
        await AuthenticateAsync();

        var boardResponse = await _client.PostAsJsonAsync("/api/v1/boards",
            new CreateBoardRequest("Board A", "Desc"));

        var board = await boardResponse.Content.ReadFromJsonAsync<IdResponse>();
        board.Should().NotBeNull();

        var boardDetails = await _client.GetFromJsonAsync<BoardDto>($"/api/v1/boards/{board.Id}");
        var columnId = boardDetails!.Columns.First().Id;

        var createTask = new CreateTaskRequest(columnId, "My Task", "My Description", "🔥");

        var createResponse = await _client.PostAsJsonAsync(
            $"/api/v1/boards/{board.Id}/tasks",
            createTask
        );

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var tasksResponse = await _client.GetAsync($"/api/v1/boards/{board.Id}/tasks");
        var tasks = await tasksResponse.Content.ReadFromJsonAsync<List<TaskDto>>();

        tasks.Should().HaveCount(1);

        tasks.Should().Contain(t =>
            t.Name == "My Task" &&
            t.Description == "My Description" &&
            t.Icon == "🔥" &&
            t.ColumnId == columnId
        );
    }

    // ------------------------------------------------------------
    // M1 — PATCH /tasks/{id}/move : reorder in same column
    // ------------------------------------------------------------
    [Fact]
    public async Task MoveTask_ShouldReorderTasksWithinSameColumn()
    {
        await AuthenticateAsync();

        var boardResponse = await _client.PostAsJsonAsync("/api/v1/boards",
            new CreateBoardRequest("Board A", "Desc"));

        var board = await boardResponse.Content.ReadFromJsonAsync<IdResponse>();
        var boardDetails = await _client.GetFromJsonAsync<BoardDto>($"/api/v1/boards/{board!.Id}");
        var columnId = boardDetails!.Columns.First().Id;

        var t1 = await CreateTask(board.Id, columnId, "T1");
        var t2 = await CreateTask(board.Id, columnId, "T2");
        var t3 = await CreateTask(board.Id, columnId, "T3");

        var moveRequest = new MoveTaskRequest(columnId, 0);
        await _client.PatchAsJsonAsync($"/api/v1/tasks/{t3.Id}/move", moveRequest);

        var tasks = await _client.GetFromJsonAsync<List<TaskDto>>(
            $"/api/v1/boards/{board.Id}/tasks");

        var ordered = tasks!.OrderBy(t => t.Order).ToList();

        ordered[0].Id.Should().Be(t3.Id);
        ordered[1].Id.Should().Be(t1.Id);
        ordered[2].Id.Should().Be(t2.Id);
    }

    private async Task<IdResponse> CreateTask(Guid boardId, Guid columnId, string name)
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/v1/boards/{boardId}/tasks",
            new CreateTaskRequest(columnId, name, null, null));

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<IdResponse>())!;
    }

    // ------------------------------------------------------------
    // M2 — PATCH /tasks/{id}/move : move to another column
    // ------------------------------------------------------------
    [Fact]
    public async Task MoveTask_ShouldMoveTaskToAnotherColumn_AndRecalculateOrder()
    {
        await AuthenticateAsync();

        // 1) Create board
        var boardResponse = await _client.PostAsJsonAsync("/api/v1/boards",
            new CreateBoardRequest("Board A", "Desc"));

        var board = await boardResponse.Content.ReadFromJsonAsync<IdResponse>();
        board.Should().NotBeNull();

        // 2) Get columns
        var boardDetails = await _client.GetFromJsonAsync<BoardDto>($"/api/v1/boards/{board.Id}");
        boardDetails.Should().NotBeNull();

        var colA = boardDetails!.Columns.First().Id;
        var colB = boardDetails!.Columns.Last().Id;

        // 3) Create tasks in colA
        var t1Response = await _client.PostAsJsonAsync(
            $"/api/v1/boards/{board.Id}/tasks",
            new CreateTaskRequest(colA, "A1", "D1", "🔥")
        );
        var t1 = await t1Response.Content.ReadFromJsonAsync<IdResponse>();
        t1.Should().NotBeNull();

        var t2Response = await _client.PostAsJsonAsync(
            $"/api/v1/boards/{board.Id}/tasks",
            new CreateTaskRequest(colA, "A2", "D2", "🔥")
        );
        var t2 = await t2Response.Content.ReadFromJsonAsync<IdResponse>();
        t2.Should().NotBeNull();

        // 4) Move t2 to colB
        var moveRequest = new MoveTaskRequest(colB, 0);
        var moveResponse = await _client.PatchAsJsonAsync(
            $"/api/v1/tasks/{t2.Id}/move",
            moveRequest
        );

        moveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 5) GET tasks and verify
        var tasks = await _client.GetFromJsonAsync<List<TaskDto>>(
            $"/api/v1/boards/{board.Id}/tasks"
        );

        tasks.Should().NotBeNull();

        var colATasks = tasks!.Where(t => t.ColumnId == colA).OrderBy(t => t.Order).ToList();
        var colBTasks = tasks!.Where(t => t.ColumnId == colB).OrderBy(t => t.Order).ToList();

        colATasks.Should().HaveCount(1);
        colATasks[0].Id.Should().Be(t1.Id);

        colBTasks.Should().HaveCount(1);
        colBTasks[0].Id.Should().Be(t2.Id);
        colBTasks[0].Order.Should().Be(0);
    }

    // ------------------------------------------------------------
    // M3 — PATCH /tasks/{id}/move : 404
    // ------------------------------------------------------------
    [Fact]
    public async Task MoveTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        await AuthenticateAsync();

        var moveRequest = new MoveTaskRequest(Guid.NewGuid(), 0);

        var response = await _client.PatchAsJsonAsync(
            $"/api/v1/tasks/{Guid.NewGuid()}/move",
            moveRequest
        );

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
