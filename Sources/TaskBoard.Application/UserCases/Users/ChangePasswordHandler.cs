using MediatR;

using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordRequest>
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IPasswordHasher _hasher;
    private readonly IUnitOfWork _uow;

    public Guid UserId { get; set; }

    public ChangePasswordHandler(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IPasswordHasher hasher,
        IUnitOfWork uow)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _hasher = hasher;
        _uow = uow;
    }

    public async Task Handle(ChangePasswordRequest request, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(UserId, ct)
            ?? throw new UnauthorizedAccessException();

        if (!_hasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new DomainException("Mot de passe actuel incorrect.");

        await _refreshTokens.RevokeAllForUserAsync(user.Id, ct);

        user.UpdatePassword(_hasher.Hash(request.NewPassword));

        await _uow.SaveChangesAsync(ct);
    }
}
