using Microsoft.Extensions.Configuration;
using TaskBoard.Infrastructure.Security;

namespace TaskBoard.Infrastructure.Tests.Security;

public class JwtTokenServiceTests
{
    [Fact]
    public void Should_Generate_Valid_JWT()
    {
        var settings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "TEST_SECRET_KEY_123456789" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" },
            { "Jwt:ExpiresMinutes", "60" }
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        var service = new JwtTokenService(config);

        var token = service.GenerateToken(Guid.NewGuid(), "test@mail.com");

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Contains(".", token);
    }
}
