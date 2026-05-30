namespace TaskBoard.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    private User() { }

    public User(string email, string passwordHash, string displayName)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        DisplayName = displayName;
    }
}
