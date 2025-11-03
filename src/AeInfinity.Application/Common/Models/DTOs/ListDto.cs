namespace AeInfinity.Application.Common.Models.DTOs;

public class ListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public UserBasicDto? Owner { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Statistics
    public int TotalItems { get; set; }
    public int PurchasedItems { get; set; }
    public int CollaboratorsCount { get; set; }
}

public class ListDetailDto : ListDto
{
    public List<CollaboratorDto> Collaborators { get; set; } = new();
    public List<ListItemDto> Items { get; set; } = new();
}

public class ListBasicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsArchived { get; set; }
}

