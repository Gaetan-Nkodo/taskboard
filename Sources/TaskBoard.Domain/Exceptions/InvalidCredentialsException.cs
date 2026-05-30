namespace TaskBoard.Domain.Exceptions;

public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Invalid credentials.") { }
}
