public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool Revoked { get; private set; }

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        Revoked = false;
    }

    public void Revoke()
    {
        Revoked = true;
    }

    public void Rotate(string newToken, DateTime newExpiresAt)
    {
        Token = newToken;
        ExpiresAt = newExpiresAt;
        Revoked = false;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
