using AeInfinity.Application.Common.Interfaces;

namespace AeInfinity.Infrastructure.Services;

/// <summary>
/// Password hashing service using BCrypt
/// </summary>
public class PasswordHasherService : IPasswordHasher
{
    /// <summary>
    /// Hash a password using BCrypt
    /// </summary>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verify a password against a hash
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}

/// <summary>
/// Static helper for database seeding
/// </summary>
public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}

