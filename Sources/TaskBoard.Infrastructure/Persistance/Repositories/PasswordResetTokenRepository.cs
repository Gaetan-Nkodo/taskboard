using Microsoft.EntityFrameworkCore;

using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Infrastructure.Persistence.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly AppDbContext _db;

    public PasswordResetTokenRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task AddAsync(PasswordResetToken token, CancellationToken ct = default)
        => _db.PasswordResetTokens.AddAsync(token, ct).AsTask();

    public Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => _db.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == token, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
