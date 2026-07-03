using Microsoft.EntityFrameworkCore;

using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Persistance.Repositories;

public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
{
    private readonly AppDbContext _db;

    public EmailVerificationTokenRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<EmailVerificationToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => _db.EmailVerificationTokens.FirstOrDefaultAsync(t => t.Token == token, ct);

    public async Task AddAsync(EmailVerificationToken token, CancellationToken ct = default)
    {
        await _db.EmailVerificationTokens.AddAsync(token, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(EmailVerificationToken token, CancellationToken ct = default)
    {
        _db.EmailVerificationTokens.Remove(token);
        await _db.SaveChangesAsync(ct);
    }
}
