using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AeInfinity.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly int _expirationHours = 24;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtSecret()));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: GetJwtIssuer(),
            audience: GetJwtAudience(),
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_expirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetTokenExpiration()
    {
        return DateTime.UtcNow.AddHours(_expirationHours);
    }

    private string GetJwtSecret()
    {
        return _configuration["Jwt:Secret"] 
            ?? throw new InvalidOperationException("JWT Secret is not configured");
    }

    private string GetJwtIssuer()
    {
        return _configuration["Jwt:Issuer"] ?? "AeInfinityApi";
    }

    private string GetJwtAudience()
    {
        return _configuration["Jwt:Audience"] ?? "AeInfinityClient";
    }
}

