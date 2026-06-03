using Microsoft.EntityFrameworkCore;

using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;
using TaskBoard.Infrastructure.Persistence;

namespace TaskBoard.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, ct);

    public async Task StoreAsync(RefreshToken token, CancellationToken ct = default)
    {
        await _db.RefreshTokens.AddAsync(token, ct);
    }

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var tokens = await _db.RefreshTokens
            .Where(r => r.UserId == userId && !r.Revoked)
            .ToListAsync(ct);

        foreach (var t in tokens)
            t.Revoke();
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
