using Microsoft.Extensions.Configuration;
using TaskBoard.Infrastructure.Security;

namespace TaskBoard.Api.Tests.Infrastructure.Security;

public class JwtTokenServiceTests
{
    [Fact]
    public void Should_Generate_Valid_JWT()
    {
        var settings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "THIS_IS_A_32_BYTE_MINIMUM_SECRET_KEY" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" },
            { "Jwt:ExpiresMinutes", "60" }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var service = new JwtTokenService(config);

        var token = service.GenerateToken(Guid.NewGuid(), "test@mail.com");

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Contains(".", token);
    }
}
