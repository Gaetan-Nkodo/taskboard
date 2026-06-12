using TaskBoard.Application.Services;

namespace TaskBoard.Api.Tests.Utils;

public class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => password;

    public bool Verify(string password, string hash) => password == hash;
}
