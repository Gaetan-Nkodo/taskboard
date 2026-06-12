using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Users;

public class ChangePasswordHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshTokens = Substitute.For<IRefreshTokenRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_ShouldUpdatePassword_AndRevokeTokens()
    {
        var user = new User("test@example.com", "oldhash", "Test");
        var userId = user.Id;

        _users.GetByIdAsync(userId).Returns(user);
        _hasher.Verify("OLD", "oldhash").Returns(true);
        _hasher.Hash("NEW").Returns("newhash");

        var handler = new ChangePasswordHandler(_users, _refreshTokens, _hasher, _uow)
        {
            UserId = userId
        };

        var request = new ChangePasswordRequest("OLD", "NEW");

        await handler.Handle(request, default);

        user.PasswordHash.Should().Be("newhash");

        await _refreshTokens.Received(1).RevokeAllForUserAsync(userId, default);
        await _uow.Received(1).SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCurrentPasswordIncorrect()
    {
        var user = new User("test@example.com", "oldhash", "Test");
        var userId = user.Id;

        _users.GetByIdAsync(userId).Returns(user);
        _hasher.Verify("BAD", "oldhash").Returns(false);

        var handler = new ChangePasswordHandler(_users, _refreshTokens, _hasher, _uow)
        {
            UserId = userId
        };

        var request = new ChangePasswordRequest("BAD", "NEW");

        var act = () => handler.Handle(request, default);

        await act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Mot de passe actuel incorrect.");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();

        _users.GetByIdAsync(userId).Returns((User?)null);

        var handler = new ChangePasswordHandler(_users, _refreshTokens, _hasher, _uow)
        {
            UserId = userId
        };

        var request = new ChangePasswordRequest("ANY", "NEW");

        var act = () => handler.Handle(request, default);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
