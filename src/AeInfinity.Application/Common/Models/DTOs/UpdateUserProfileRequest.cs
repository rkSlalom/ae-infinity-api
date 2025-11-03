namespace AeInfinity.Application.Common.Models.DTOs;

/// <summary>
/// Request model for updating user profile
/// </summary>
public class UpdateUserProfileRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}
