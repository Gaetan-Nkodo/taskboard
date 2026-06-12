using Microsoft.Extensions.DependencyInjection;

using TaskBoard.Api.Tests.Fixtures;

public sealed class ApiScope : IDisposable
{
    private readonly IServiceScope _scope;
    public AppDbContext Db { get; }

    public ApiScope(ApiFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // IMPORTANT : on force EF Core à ne rien traquer
        Db.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        Db.Dispose();
        _scope.Dispose();
    }
}
