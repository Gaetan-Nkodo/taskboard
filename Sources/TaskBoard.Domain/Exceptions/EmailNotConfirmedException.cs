using TaskBoard.Domain.Exceptions;

public class EmailNotConfirmedException : DomainException
{
    public EmailNotConfirmedException()
        : base("Email not confirmed.") { }
}
