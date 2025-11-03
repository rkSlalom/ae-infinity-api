using AeInfinity.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Infrastructure.Services;

public class ListPermissionService : IListPermissionService
{
    private readonly IApplicationDbContext _context;

    public ListPermissionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CanUserAccessListAsync(Guid userId, Guid listId, CancellationToken cancellationToken = default)
    {
        // User can access if they are owner or collaborator
        var list = await _context.Lists
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == listId, cancellationToken);

        if (list == null)
            return false;

        // Check if owner
        if (list.OwnerId == userId)
            return true;

        // Check if collaborator
        var isCollaborator = await _context.UserToLists
            .AsNoTracking()
            .AnyAsync(utl => utl.ListId == listId && utl.UserId == userId && !utl.IsPending, cancellationToken);

        return isCollaborator;
    }

    public async Task<bool> CanUserEditListAsync(Guid userId, Guid listId, CancellationToken cancellationToken = default)
    {
        // Only owner can edit list details in prototype
        return await IsUserListOwnerAsync(userId, listId, cancellationToken);
    }

    public async Task<bool> CanUserDeleteListAsync(Guid userId, Guid listId, CancellationToken cancellationToken = default)
    {
        // Only owner can delete list
        return await IsUserListOwnerAsync(userId, listId, cancellationToken);
    }

    public async Task<bool> CanUserArchiveListAsync(Guid userId, Guid listId, CancellationToken cancellationToken = default)
    {
        // Only owner can archive/unarchive list
        return await IsUserListOwnerAsync(userId, listId, cancellationToken);
    }

    public async Task<string?> GetUserRoleForListAsync(Guid userId, Guid listId, CancellationToken cancellationToken = default)
    {
        // Check if owner first
        var list = await _context.Lists
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == listId, cancellationToken);

        if (list?.OwnerId == userId)
            return "Owner";

        // Check collaborator role
        var userToList = await _context.UserToLists
            .Include(utl => utl.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(utl => utl.ListId == listId && utl.UserId == userId, cancellationToken);

        return userToList?.Role.Name;
    }

    public async Task<bool> IsUserListOwnerAsync(Guid userId, Guid listId, CancellationToken cancellationToken = default)
    {
        return await _context.Lists
            .AsNoTracking()
            .AnyAsync(l => l.Id == listId && l.OwnerId == userId, cancellationToken);
    }
}

