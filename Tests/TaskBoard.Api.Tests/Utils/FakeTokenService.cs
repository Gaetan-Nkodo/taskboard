using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using TaskBoard.Application.Services;

namespace TaskBoard.Api.Tests.Utils;

public sealed class FakeTokenService : ITokenService
{
    private readonly IConfiguration _config;

    public FakeTokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(Guid userId, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("sub", userId.ToString()),               // 🔥 obligatoire
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

    public Task<string> GenerateRefreshToken(Guid userId)
        => Task.FromResult("TEST_REFRESH_TOKEN");
}
