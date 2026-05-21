using FluentAssertions;

using Microsoft.Extensions.Configuration;

using NSubstitute;

using TaskBoard.Infrastructure.Security;

namespace TaskBoard.Infrastructure.Tests.Security;

public class JwtTokenServiceTests
{
    [Fact]
    public void GenerateToken_ShouldReturnNonEmptyToken()
    {
        var config = Substitute.For<IConfiguration>();
        config["Jwt:Key"].Returns("THIS_IS_A_32_BYTE_MINIMUM_SECRET_KEY_1234");
        var service = new JwtTokenService(config);

        var token = service.GenerateToken(Guid.NewGuid(), "user@example.com");

        token.Should().NotBeNullOrWhiteSpace();
        token.Should().Contain(".");
    }
}
