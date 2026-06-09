public class ConsoleEmailService : IEmailService
{
    public Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        Console.WriteLine($"[EMAIL] To: {email} — Reset link: {resetLink}");
        return Task.CompletedTask;
    }
}
