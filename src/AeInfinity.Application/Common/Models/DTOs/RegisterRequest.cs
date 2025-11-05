using System.ComponentModel.DataAnnotations;

namespace AeInfinity.Application.Common.Models.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Display name is required")]
    [MinLength(2, ErrorMessage = "Display name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Display name must not exceed 100 characters")]
    public string DisplayName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; } = string.Empty;
}

