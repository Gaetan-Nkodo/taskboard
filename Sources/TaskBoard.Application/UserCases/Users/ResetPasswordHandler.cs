using MediatR;

using TaskBoard.Application.Common.Emails;
using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordRequest>
{
    private readonly IPasswordResetTokenRepository _tokens;
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IPasswordHasher _hasher;
    private readonly IUnitOfWork _uow;
    private readonly IEmailSender _emailSender;

    public ResetPasswordHandler(
        IPasswordResetTokenRepository tokens,
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IPasswordHasher hasher,
        IUnitOfWork uow,
        IEmailSender emailSender)
    {
        _tokens = tokens;
        _users = users;
        _refreshTokens = refreshTokens;
        _hasher = hasher;
        _uow = uow;
        _emailSender = emailSender;
    }

    public async Task Handle(ResetPasswordRequest request, CancellationToken ct)
    {
        var token = await _tokens.GetByTokenAsync(request.Token, ct);
        if (token is null || !token.IsValid())
            throw new InvalidCredentialsException();

        var user = await _users.GetByIdAsync(token.UserId, ct)
            ?? throw new InvalidCredentialsException();

        await _refreshTokens.RevokeAllForUserAsync(user.Id, ct);

        token.MarkUsed();

        user.UpdatePassword(_hasher.Hash(request.NewPassword));

        await _uow.SaveChangesAsync(ct);

        await _emailSender.SendAsync(
            user.Email,
            "Mot de passe mis à jour",
            EmailTemplates.PasswordChanged()
        );
    }
}
