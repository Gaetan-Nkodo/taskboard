using TaskBoard.Domain.Entities;
public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string resetLink);
}
