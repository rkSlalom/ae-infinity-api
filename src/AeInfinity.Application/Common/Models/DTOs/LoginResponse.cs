using System.ComponentModel.DataAnnotations;

namespace AeInfinity.Application.Common.Models.DTOs;

public class LoginResponse
{
    [Required]
    public string Token { get; set; } = string.Empty;
    
    [Required]
    public DateTime ExpiresAt { get; set; }
    
    [Required]
    public UserDto User { get; set; } = null!;
}

