using Microsoft.Extensions.DependencyInjection;

using TaskBoard.Api.Tests.Fixtures;
using TaskBoard.Infrastructure.Persistence;

namespace TaskBoard.Api.Tests.Utils;

public sealed class ApiScope : IDisposable
{
    private readonly IServiceScope _scope;

    public AppDbContext Db { get; }

    public ApiScope(ApiFactory factory)
    {
        // Récupère le ServiceProvider du WebApplicationFactory
        _scope = factory.Services.CreateScope();

        // Récupère le DbContext
        Db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }
}
