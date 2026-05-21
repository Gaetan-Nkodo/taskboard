using FluentAssertions;

using TaskBoard.Infrastructure.Security;

namespace TaskBoard.Infrastructure.Tests.Security;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher = new();

    [Fact]
    public void Hash_ShouldReturnNonEmptyString()
    {
        var password = "P@ssw0rd!";
        var hash = _hasher.Hash(password);

        hash.Should().NotBeNullOrWhiteSpace();
        hash.Should().NotBe(password);
    }

    [Fact]
    public void Verify_ShouldReturnTrue_WhenPasswordMatchesHash()
    {
        var password = "P@ssw0rd!";
        var hash = _hasher.Hash(password);

        var result = _hasher.Verify(password, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_ShouldReturnFalse_WhenPasswordDoesNotMatchHash()
    {
        var hash = _hasher.Hash("P@ssw0rd!");

        var result = _hasher.Verify("wrong", hash);

        result.Should().BeFalse();
    }
}
