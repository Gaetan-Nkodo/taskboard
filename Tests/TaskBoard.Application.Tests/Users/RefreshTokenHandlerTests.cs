using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Users;

public class RefreshTokenHandlerTests
{
    private readonly IRefreshTokenRepository _repo = Substitute.For<IRefreshTokenRepository>();
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly ITokenService _tokens = Substitute.For<ITokenService>();

    [Fact]
    public async Task Handle_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        var userId = Guid.NewGuid();
        var stored = new RefreshToken(userId, "OLD_REFRESH", DateTime.UtcNow.AddMinutes(10));

        _repo.GetByTokenAsync("OLD_REFRESH", Arg.Any<CancellationToken>())
             .Returns(stored);

        var user = new User("user@example.com", "hash", "Test User");
        typeof(User).GetProperty("Id")!.SetValue(user, userId);

        _users.GetByIdAsync(userId).Returns(user);

        _tokens.GenerateToken(userId, user.Email).Returns("NEW_ACCESS");
        _tokens.GenerateRefreshToken(userId).Returns("NEW_REFRESH");

        var handler = new RefreshTokenHandler(_repo, _users, _tokens);

        var result = await handler.Handle(new RefreshTokenRequest("OLD_REFRESH"));

        result.AccessToken.Should().Be("NEW_ACCESS");
        result.RefreshToken.Should().Be("NEW_REFRESH");
        result.User.Email.Should().Be("user@example.com");

        stored.Revoked.Should().BeTrue();
        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenRefreshTokenExpired()
    {
        var stored = new RefreshToken(Guid.NewGuid(), "OLD", DateTime.UtcNow.AddMinutes(-1));

        _repo.GetByTokenAsync("OLD", Arg.Any<CancellationToken>())
             .Returns(stored);

        var handler = new RefreshTokenHandler(_repo, _users, _tokens);

        var act = () => handler.Handle(new RefreshTokenRequest("OLD"));

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenRefreshTokenRevoked()
    {
        var stored = new RefreshToken(Guid.NewGuid(), "OLD", DateTime.UtcNow.AddMinutes(10));
        stored.Revoke();

        _repo.GetByTokenAsync("OLD", Arg.Any<CancellationToken>())
             .Returns(stored);

        var handler = new RefreshTokenHandler(_repo, _users, _tokens);

        var act = () => handler.Handle(new RefreshTokenRequest("OLD"));

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserIsInactive()
    {
        var userId = Guid.NewGuid();
        var stored = new RefreshToken(userId, "OLD", DateTime.UtcNow.AddMinutes(10));

        _repo.GetByTokenAsync("OLD", Arg.Any<CancellationToken>())
             .Returns(stored);

        var user = new User("user@example.com", "hash", "Test User");
        typeof(User).GetProperty("Id")!.SetValue(user, userId);
        user.Deactivate();

        _users.GetByIdAsync(userId).Returns(user);

        var handler = new RefreshTokenHandler(_repo, _users, _tokens);

        var act = () => handler.Handle(new RefreshTokenRequest("OLD"));

        await act.Should().ThrowAsync<AccountDisabledException>();
    }
}
