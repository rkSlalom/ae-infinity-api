using System.ComponentModel.DataAnnotations;

namespace AeInfinity.Application.Common.Models.DTOs;

/// <summary>
/// Limited user information for public profiles (privacy-respecting view for collaborators)
/// </summary>
public class PublicUserProfileDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;
    
    [Url]
    [MaxLength(500)]
    public string? AvatarUrl { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
}

