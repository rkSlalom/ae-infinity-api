namespace AeInfinity.Application.Common.Models.DTOs;

public class UserStatsDto
{
    public int TotalListsOwned { get; set; }
    public int TotalListsShared { get; set; }
    public int TotalItemsCreated { get; set; }
    public int TotalItemsPurchased { get; set; }
    public int TotalActiveCollaborations { get; set; }
    public DateTime? LastActivityAt { get; set; }
}

