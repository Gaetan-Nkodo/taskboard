namespace TaskBoard.Domain.Exceptions;

public sealed class UserNotFoundException : DomainException
{
    public UserNotFoundException()
        : base("User not found.") { }
}
