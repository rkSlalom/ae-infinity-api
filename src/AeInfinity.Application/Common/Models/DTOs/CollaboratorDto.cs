namespace AeInfinity.Application.Common.Models.DTOs;

public class CollaboratorDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserDisplayName { get; set; } = string.Empty;
    public string? UserAvatarUrl { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public RoleDto? Role { get; set; }
    public bool IsPending { get; set; }
    public DateTime InvitedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public Guid InvitedBy { get; set; }
    public string InvitedByDisplayName { get; set; } = string.Empty;
}

