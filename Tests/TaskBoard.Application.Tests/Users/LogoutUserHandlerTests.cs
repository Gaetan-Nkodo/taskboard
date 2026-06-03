using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Users;

public class LogoutUserHandlerTests
{
    private readonly IRefreshTokenRepository _repo = Substitute.For<IRefreshTokenRepository>();

    [Fact]
    public async Task Handle_ShouldRevokeAllTokens_AndSaveChanges()
    {
        var handler = new LogoutUserHandler(_repo);
        var userId = Guid.NewGuid();

        await handler.Handle(userId);

        await _repo.Received(1).RevokeAllForUserAsync(userId, Arg.Any<CancellationToken>());
        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
