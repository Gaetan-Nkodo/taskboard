using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.Services;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

public class LoginUserHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokens = Substitute.For<ITokenService>();
    private readonly IRefreshTokenRepository _refreshTokens = Substitute.For<IRefreshTokenRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private LoginUserHandler CreateHandler()
        => new LoginUserHandler(_users, _hasher, _tokens, _refreshTokens, _uow);

    [Fact]
    public async Task Handle_ShouldReturnAccessAndRefreshTokens_WhenCredentialsAreValid()
    {
        var request = new LoginUserRequest("user@example.com", "P@ssw0rd!");
        var user = new User("user@example.com", "hashed", "Test User");

        _users.GetByEmailAsync(request.Email).Returns(user);
        _hasher.Verify(request.Password, user.PasswordHash).Returns(true);

        _tokens.GenerateToken(user.Id, user.Email).Returns("ACCESS_TOKEN");
        _tokens.GenerateRefreshToken(user.Id).Returns("REFRESH_TOKEN");

        var handler = CreateHandler();

        var result = await handler.Handle(request);

        result.AccessToken.Should().Be("ACCESS_TOKEN");
        result.RefreshToken.Should().Be("REFRESH_TOKEN");
        result.User.Email.Should().Be("user@example.com");

        // 🔥 Vérifie que la transaction est commit
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        var request = new LoginUserRequest("missing@example.com", "pwd");

        _users.GetByEmailAsync(request.Email).Returns((User?)null);

        var handler = CreateHandler();

        var act = () => handler.Handle(request);

        await act.Should().ThrowAsync<InvalidCredentialsException>();

        // 🔥 Aucun commit ne doit être fait
        await _uow.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPasswordInvalid()
    {
        var request = new LoginUserRequest("user@example.com", "wrong");
        var user = new User("user@example.com", "hashed", "Test User");

        _users.GetByEmailAsync(request.Email).Returns(user);
        _hasher.Verify(request.Password, user.PasswordHash).Returns(false);

        var handler = CreateHandler();

        var act = () => handler.Handle(request);

        await act.Should().ThrowAsync<InvalidCredentialsException>();

        // 🔥 Aucun commit ne doit être fait
        await _uow.DidNotReceive().SaveChangesAsync();
    }
}
