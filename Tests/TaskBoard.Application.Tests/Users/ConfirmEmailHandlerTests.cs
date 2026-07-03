using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.UserCases.Users;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Users;

public class ConfirmEmailHandlerTests
{
    private readonly IEmailVerificationTokenRepository _tokens = Substitute.For<IEmailVerificationTokenRepository>();
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();

    [Fact]
    public async Task Handle_ShouldConfirmEmail_WhenTokenIsValid()
    {
        var user = new User("test@example.com", "hash", "Gaetan");
        var token = new EmailVerificationToken(user.Id, "abc", DateTime.UtcNow.AddHours(1));

        _tokens.GetByTokenAsync("abc").Returns(token);
        _users.GetByIdAsync(user.Id).Returns(user);

        var handler = new ConfirmEmailHandler(_tokens, _users);

        await handler.Handle("abc", CancellationToken.None);

        user.EmailConfirmed.Should().BeTrue();
        await _tokens.Received(1).RemoveAsync(token);
        await _users.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenTokenIsExpired()
    {
        var token = new EmailVerificationToken(Guid.NewGuid(), "abc", DateTime.UtcNow.AddHours(-1));
        _tokens.GetByTokenAsync("abc").Returns(token);

        var handler = new ConfirmEmailHandler(_tokens, _users);

        var act = () => handler.Handle("abc", CancellationToken.None);

        await act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Invalid or expired token.");
    }
}
