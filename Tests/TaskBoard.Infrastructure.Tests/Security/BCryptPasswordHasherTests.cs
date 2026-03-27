using TaskBoard.Infrastructure.Security;

namespace TaskBoard.Infrastructure.Tests.Security;

public class BCryptPasswordHasherTests
{
    [Fact]
    public void Hash_Should_Generate_Different_Hashes_For_Same_Password()
    {
        var hasher = new BCryptPasswordHasher();

        var hash1 = hasher.Hash("password");
        var hash2 = hasher.Hash("password");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Verify_Should_Return_True_For_Valid_Password()
    {
        var hasher = new BCryptPasswordHasher();

        var hash = hasher.Hash("password");

        Assert.True(hasher.Verify("password", hash));
    }

    [Fact]
    public void Verify_Should_Return_False_For_Invalid_Password()
    {
        var hasher = new BCryptPasswordHasher();

        var hash = hasher.Hash("password");

        Assert.False(hasher.Verify("wrong", hash));
    }
}
