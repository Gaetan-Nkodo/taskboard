using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using TaskBoard.Application.Services;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Infrastructure.Security;

public sealed class JwtTokenService : ITokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(Guid userId, string email)
    {
        var key = _config["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Missing Jwt:Key in configuration.");

        var issuer = _config["Jwt:Issuer"] ?? "TaskBoard";
        var audience = _config["Jwt:Audience"] ?? "TaskBoardUsers";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("sub", userId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email)
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<string> GenerateRefreshToken(Guid userId)
    {
        // 🔥 IMPORTANT :
        // On NE STOCKE PLUS le refresh token ici.
        // RefreshTokenHandler gère la rotation et le stockage.
        var token = Guid.NewGuid().ToString("N");
        return Task.FromResult(token);
    }

    public Task<(Guid UserId, string Email)> ValidateAccessTokenAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var userId = Guid.Parse(jwt.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
        var email = jwt.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;

        return Task.FromResult((userId, email));
    }
}
