using AeInfinity.Domain.Entities;

namespace AeInfinity.Application.Common.Interfaces;

public interface IListItemRepository
{
    // CRUD Operations
    Task<ListItem> CreateAsync(ListItem item, CancellationToken cancellationToken = default);
    Task<ListItem?> GetByIdAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<List<ListItem>> GetByListIdAsync(Guid listId, GetItemsFilter? filter = null, CancellationToken cancellationToken = default);
    Task UpdateAsync(ListItem item, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid itemId, CancellationToken cancellationToken = default);
    
    // Specialized Operations
    Task<bool> ExistsAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<int> GetNextPositionAsync(Guid listId, CancellationToken cancellationToken = default);
    Task ReorderAsync(List<ItemPosition> positions, CancellationToken cancellationToken = default);
    
    // Autocomplete
    Task<List<AutocompleteSuggestion>> GetAutocompleteAsync(Guid userId, string query, CancellationToken cancellationToken = default);
}

public class GetItemsFilter
{
    public Guid? CategoryId { get; set; }
    public bool? IsPurchased { get; set; }
    public string SortBy { get; set; } = "position"; // "name", "createdAt", "category"
    public string SortOrder { get; set; } = "asc"; // "asc", "desc"
}

public class ItemPosition
{
    public Guid ItemId { get; set; }
    public int Position { get; set; }
}

public class AutocompleteSuggestion
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int Frequency { get; set; }
}

