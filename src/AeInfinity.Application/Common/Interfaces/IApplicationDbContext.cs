using AeInfinity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // DbSets will be added in Phase 2
    // DbSet<User> Users { get; }
    // DbSet<Role> Roles { get; }
    // DbSet<List> Lists { get; }
    // DbSet<UserToList> UserToLists { get; }
    // DbSet<Category> Categories { get; }
    // DbSet<ListItem> ListItems { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

