namespace AeInfinity.Application.Common.Models.DTOs;

/// <summary>
/// DTO for list collaboration invitation
/// </summary>
public class InvitationDto
{
    public Guid Id { get; set; }
    public Guid ListId { get; set; }
    public string ListName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public Guid InvitedBy { get; set; }
    public string InvitedByDisplayName { get; set; } = string.Empty;
    public DateTime InvitedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public bool IsPending { get; set; }
}

