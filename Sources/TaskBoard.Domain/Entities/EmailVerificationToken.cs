namespace TaskBoard.Domain.Entities;

public class EmailVerificationToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }

    private EmailVerificationToken() { }

    public EmailVerificationToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
}
