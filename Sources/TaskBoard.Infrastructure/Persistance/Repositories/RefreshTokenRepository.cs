using Microsoft.EntityFrameworkCore;

using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task StoreAsync(RefreshToken token)
    {
        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetAsync(string token)
    {
        return await _db.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Token == token && !x.Revoked);
    }

    public async Task RevokeAsync(string token)
    {
        var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        if (rt != null)
        {
            rt.Revoke();
            await _db.SaveChangesAsync();
        }
    }
}
