using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using TaskBoard.Application.Services;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Api.Tests.Utils;

public sealed class FakeTokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly IRefreshTokenRepository _refreshRepo;

    public FakeTokenService(IConfiguration config, IRefreshTokenRepository refreshRepo)
    {
        _config = config;
        _refreshRepo = refreshRepo;
    }

    public string GenerateToken(Guid userId, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("sub", userId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshToken(Guid userId)
    {
        var token = Guid.NewGuid().ToString("N");

        var refresh = new RefreshToken(
            userId,
            token,
            DateTime.UtcNow.AddHours(1)
        );

        await _refreshRepo.StoreAsync(refresh);
        await _refreshRepo.SaveChangesAsync();

        return token;
    }

    public Task<(Guid UserId, string Email)> ValidateAccessTokenAsync(string token)
    {
        return Task.FromResult((Guid.NewGuid(), "fake@test.com"));
    }
}
