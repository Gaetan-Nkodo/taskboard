using FluentAssertions;
using TaskBoard.Domain.Entities;
using Xunit;

namespace TaskBoard.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldInitializeUserCorrectly()
    {
        // Arrange
        var email = "user@example.com";
        var hash = "hashed-password";

        // Act
        var user = new User(email, hash);

        // Assert
        user.Id.Should().NotBe(Guid.Empty);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(hash);
    }

    [Fact]
    public void Constructor_ShouldAcceptAnyEmailString()
    {
        // Arrange
        var email = "invalid-email";

        // Act
        var user = new User(email, "hash");

        // Assert
        user.Email.Should().Be(email);
    }
}
