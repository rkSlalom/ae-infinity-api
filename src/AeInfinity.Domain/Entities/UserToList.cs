using AeInfinity.Domain.Common;

namespace AeInfinity.Domain.Entities;

/// <summary>
/// Junction table managing list collaboration and permissions
/// </summary>
public class UserToList : BaseAuditableEntity
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid InvitedBy { get; set; }
    public DateTime InvitedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public bool IsPending { get; set; }

    // Navigation Properties
    public List List { get; set; } = null!;
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public User InvitedByUser { get; set; } = null!;
}

