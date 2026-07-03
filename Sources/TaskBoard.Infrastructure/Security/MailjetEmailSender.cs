using System.Threading;
using System.Threading.Tasks;

using Mailjet.Client;
using Mailjet.Client.Resources;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json.Linq;

namespace TaskBoard.Infrastructure.Security;

public class MailjetEmailSender : IEmailSender
{
    private readonly MailjetClient _client;
    private readonly string _from;

    public MailjetEmailSender(IConfiguration config)
    {
        var apiKey = config["Email:ApiKey"];
        var apiSecret = config["Email:ApiSecret"];
        _from = config["Email:From"] ?? "noreply@taskboard.app";

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
            throw new InvalidOperationException("Mailjet API credentials missing.");

        _client = new MailjetClient(apiKey, apiSecret);
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        var request = new MailjetRequest
        {
            Resource = Send.Resource
        }
        .Property(Send.Messages, new JArray {
            new JObject {
                { "From", new JObject { { "Email", _from } } },
                { "To", new JArray {
                    new JObject { { "Email", to } }
                }},
                { "Subject", subject },
                { "HTMLPart", body }
            }
        });

        await _client.PostAsync(request);
    }
}
