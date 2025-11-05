using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Infrastructure.Persistence.Repositories;

public class ListItemRepository : IListItemRepository
{
    private readonly ApplicationDbContext _context;

    public ListItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ListItem> CreateAsync(ListItem item, CancellationToken cancellationToken = default)
    {
        _context.ListItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Load navigation properties
        await _context.Entry(item)
            .Reference(i => i.Category)
            .LoadAsync(cancellationToken);
        
        await _context.Entry(item)
            .Reference(i => i.Creator)
            .LoadAsync(cancellationToken);
        
        return item;
    }

    public async Task<ListItem?> GetByIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await _context.ListItems
            .Include(i => i.Category)
            .Include(i => i.Creator)
            .Include(i => i.PurchasedByUser)
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);
    }

    public async Task<List<ListItem>> GetByListIdAsync(Guid listId, GetItemsFilter? filter = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ListItems
            .Include(i => i.Category)
            .Include(i => i.Creator)
            .Include(i => i.PurchasedByUser)
            .Where(i => i.ListId == listId);
        
        // Apply filters
        if (filter?.CategoryId != null)
            query = query.Where(i => i.CategoryId == filter.CategoryId);
        
        if (filter?.IsPurchased != null)
            query = query.Where(i => i.IsPurchased == filter.IsPurchased);
        
        // Apply sorting
        query = filter?.SortBy switch
        {
            "name" => filter.SortOrder == "desc" 
                ? query.OrderByDescending(i => i.Name)
                : query.OrderBy(i => i.Name),
            "createdAt" => filter.SortOrder == "desc"
                ? query.OrderByDescending(i => i.CreatedAt)
                : query.OrderBy(i => i.CreatedAt),
            "category" => filter.SortOrder == "desc"
                ? query.OrderByDescending(i => i.Category != null ? i.Category.Name : "")
                : query.OrderBy(i => i.Category != null ? i.Category.Name : ""),
            _ => filter?.SortOrder == "desc"
                ? query.OrderByDescending(i => i.Position)
                : query.OrderBy(i => i.Position)
        };
        
        return await query.ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(ListItem item, CancellationToken cancellationToken = default)
    {
        _context.ListItems.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = await GetByIdAsync(itemId, cancellationToken);
        if (item != null)
        {
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(item, cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await _context.ListItems.AnyAsync(i => i.Id == itemId, cancellationToken);
    }

    public async Task<int> GetNextPositionAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        var maxPosition = await _context.ListItems
            .Where(i => i.ListId == listId)
            .MaxAsync(i => (int?)i.Position, cancellationToken);
        
        return (maxPosition ?? -1) + 1;
    }

    public async Task ReorderAsync(List<ItemPosition> positions, CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            foreach (var itemPosition in positions)
            {
                var item = await _context.ListItems.FindAsync(new object[] { itemPosition.ItemId }, cancellationToken);
                if (item != null)
                {
                    item.Position = itemPosition.Position;
                    item.UpdatedAt = DateTime.UtcNow;
                }
            }
            
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<List<AutocompleteSuggestion>> GetAutocompleteAsync(Guid userId, string query, CancellationToken cancellationToken = default)
    {
        var results = await _context.ListItems
            .Where(i => i.CreatedBy == userId && i.Name.Contains(query))
            .GroupBy(i => new { i.Name, i.Quantity, i.Unit, i.CategoryId })
            .Select(g => new AutocompleteSuggestion
            {
                Name = g.Key.Name,
                Quantity = g.Key.Quantity,
                Unit = g.Key.Unit,
                CategoryId = g.Key.CategoryId,
                CategoryName = g.First().Category != null ? g.First().Category.Name : null,
                Frequency = g.Count()
            })
            .OrderByDescending(s => s.Frequency)
            .ThenBy(s => s.Name)
            .Take(10)
            .ToListAsync(cancellationToken);
        
        return results;
    }
}

