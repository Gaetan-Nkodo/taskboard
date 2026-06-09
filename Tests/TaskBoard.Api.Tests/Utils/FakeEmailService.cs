namespace TaskBoard.Api.Tests.Utils;

public class FakeEmailService : IEmailService
{
    public List<(string Email, string Link)> Sent = new();

    public Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        Sent.Add((email, resetLink));
        return Task.CompletedTask;
    }
}
