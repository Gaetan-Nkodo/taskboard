using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskBoard.IntegrationTests.Database;
using TaskBoard.IntegrationTests.Setup;
using Xunit;

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

        var body = await create.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var id = Guid.Parse(body!["id"].ToString()!);

        var get = await _client.GetAsync($"/boards/{id}");
        get.EnsureSuccessStatusCode();
    }
}
