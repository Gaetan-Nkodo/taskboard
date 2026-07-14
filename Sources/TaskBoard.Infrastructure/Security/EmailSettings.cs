namespace TaskBoard.Infrastructure.Security;

public class EmailSettings
{
    public string Mode { get; set; } = "Smtp";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1025;
    public bool UseSsl { get; set; } = false;
    public string From { get; set; } = "dev@localhost";
    public required string User { get; set; }
    public required string Password { get; set; }
}
