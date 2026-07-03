using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Users;

public class ResetPasswordHandlerTests
{
    private readonly IPasswordResetTokenRepository _tokens = Substitute.For<IPasswordResetTokenRepository>();
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshTokens = Substitute.For<IRefreshTokenRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly IEmailSender _emailSender = Substitute.For<IEmailSender>();

    [Fact]
    public async Task Handle_ShouldUpdatePassword_AndRevokeTokens_AndSendEmail()
    {
        var user = new User("test@example.com", "oldhash", "Test");
        var token = new PasswordResetToken(user.Id, "TOKEN", DateTime.UtcNow.AddMinutes(10));

        _tokens.GetByTokenAsync("TOKEN").Returns(token);
        _users.GetByIdAsync(user.Id).Returns(user);
        _hasher.Hash("NEW").Returns("newhash");

        var handler = new ResetPasswordHandler(
            _tokens, _users, _refreshTokens, _hasher, _uow, _emailSender);

        var request = new ResetPasswordRequest("TOKEN", "NEW");

        await handler.Handle(request, default);

        user.PasswordHash.Should().Be("newhash");
        token.Used.Should().BeTrue();

        await _refreshTokens.Received(1).RevokeAllForUserAsync(user.Id, default);
        await _uow.Received(1).SaveChangesAsync(default);

        await _emailSender.Received(1).SendAsync(
            "test@example.com",
            Arg.Any<string>(),
            Arg.Any<string>(),
            default
        );
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenTokenInvalid()
    {
        _tokens.GetByTokenAsync("BAD").Returns((PasswordResetToken?)null);

        var handler = new ResetPasswordHandler(
            _tokens, _users, _refreshTokens, _hasher, _uow, _emailSender);

        var act = () => handler.Handle(new ResetPasswordRequest("BAD", "NEW"), default);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }
}
