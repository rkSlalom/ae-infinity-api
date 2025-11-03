using AeInfinity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<List> Lists { get; }
    DbSet<UserToList> UserToLists { get; }
    DbSet<Category> Categories { get; }
    DbSet<ListItem> ListItems { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

