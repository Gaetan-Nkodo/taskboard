using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskBoard.Api.Tests.Integration.Setup;
using TaskBoard.Application.DTOs;
using Xunit;

namespace TaskBoard.Api.Tests.Integration.Boards;

public class BoardsTests : IClassFixture<SqlServerContainerFixture>
{
    private readonly HttpClient _client;

    public BoardsTests(SqlServerContainerFixture fixture)
    {
        var factory = new ApiFactory(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_Create_And_Get_Board()
    {
        var request = new { name = "Board A", description = "Desc" };

        var create = await _client.PostAsJsonAsync("/boards", request);
        create.EnsureSuccessStatusCode();

        var created = await create.Content.ReadFromJsonAsync<BoardDto>()
                      ?? throw new Exception("Board creation response null");

        var get = await _client.GetAsync($"/boards/{created.Id}");
        get.EnsureSuccessStatusCode();

        var board = await get.Content.ReadFromJsonAsync<BoardDto>()
                    ?? throw new Exception("Board response null");

        Assert.Equal("Board A", board.Name);
    }
}
