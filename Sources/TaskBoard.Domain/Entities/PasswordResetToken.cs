namespace TaskBoard.Domain.Entities;

public class PasswordResetToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public bool Used { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private PasswordResetToken() { }

    public PasswordResetToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public bool IsValid() => !Used && ExpiresAt > DateTime.UtcNow;

    public void MarkUsed() => Used = true;
}
