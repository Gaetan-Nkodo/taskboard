using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Services;

public interface ITokenService
{
    string GenerateToken(Guid userId, string email);
    Task<string> GenerateRefreshToken(Guid userId);
}
