using AeInfinity.Domain.Common;

namespace AeInfinity.Domain.Entities;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User : BaseAuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string EmailNormalized { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsEmailVerified { get; set; }
    public string? EmailVerificationToken { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetExpiresAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation Properties
    public ICollection<List> OwnedLists { get; set; } = new List<List>();
    public ICollection<UserToList> ListCollaborations { get; set; } = new List<UserToList>();
    public ICollection<ListItem> CreatedItems { get; set; } = new List<ListItem>();
    public ICollection<Category> CustomCategories { get; set; } = new List<Category>();
}

