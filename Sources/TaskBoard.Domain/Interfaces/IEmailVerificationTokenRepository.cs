using TaskBoard.Domain.Entities;

public interface IEmailVerificationTokenRepository
{
    Task<EmailVerificationToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task AddAsync(EmailVerificationToken token, CancellationToken ct = default);
    Task RemoveAsync(EmailVerificationToken token, CancellationToken ct = default);
    Task<EmailVerificationToken?> GetLatestForUserAsync(Guid userId, CancellationToken ct = default);

}
