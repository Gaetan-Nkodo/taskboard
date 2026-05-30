using TaskBoard.Domain.Entities;

namespace TaskBoard.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task StoreAsync(RefreshToken token);
    Task<RefreshToken?> GetAsync(string token);
    Task RevokeAsync(string token);
}
