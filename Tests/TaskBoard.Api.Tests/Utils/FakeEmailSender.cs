namespace TaskBoard.Api.Tests.Utils;

public class FakeEmailSender : IEmailSender
{
    public List<(string To, string Subject, string Body)> Sent { get; } = new();

    public Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        Sent.Add((to, subject, body));
        return Task.CompletedTask;
    }

    public void Reset() => Sent.Clear();
}
