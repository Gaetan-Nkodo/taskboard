namespace TaskBoard.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid UserId { get; private set; }

    public string Token { get; private set; } = default!;

    public DateTime ExpiresAt { get; private set; }

    // 🔥 Doit s’appeler Revoked pour correspondre à AppDbContext
    public bool Revoked { get; private set; }

    private RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        Revoked = false;
    }

    public void Revoke()
    {
        Revoked = true;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
