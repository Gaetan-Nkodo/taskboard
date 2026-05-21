namespace TaskBoard.Application.Services;

public interface ITokenService
{
    string GenerateToken(Guid userId, string email);
}
