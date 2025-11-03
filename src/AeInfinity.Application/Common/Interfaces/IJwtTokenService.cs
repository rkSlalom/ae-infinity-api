using AeInfinity.Domain.Entities;

namespace AeInfinity.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    DateTime GetTokenExpiration();
}

