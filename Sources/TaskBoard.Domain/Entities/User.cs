public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;

    public bool IsActive { get; private set; } = true;

    private User() { } // EF Core

    public User(string email, string passwordHash, string displayName)
    {
        Email = email;
        PasswordHash = passwordHash;
        DisplayName = displayName;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
