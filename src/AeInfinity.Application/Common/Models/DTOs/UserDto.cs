using System.ComponentModel.DataAnnotations;

namespace AeInfinity.Application.Common.Models.DTOs;

public class UserDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;
    
    [Url]
    [MaxLength(500)]
    public string? AvatarUrl { get; set; }
    
    [Required]
    public bool IsEmailVerified { get; set; }
    
    public DateTime? LastLoginAt { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
}

public class UserBasicDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    [MinLength(1)]
    public string DisplayName { get; set; } = string.Empty;
    
    [Url]
    [MaxLength(500)]
    public string? AvatarUrl { get; set; }
}

