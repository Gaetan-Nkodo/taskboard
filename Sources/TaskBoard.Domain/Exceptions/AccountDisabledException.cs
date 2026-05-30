namespace TaskBoard.Domain.Exceptions;

public sealed class AccountDisabledException : Exception
{
    public AccountDisabledException()
        : base("Account disabled.") { }
}
