using System.Reflection;
using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Common;
using AeInfinity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets will be added in Phase 2
    // public DbSet<User> Users => Set<User>();
    // public DbSet<Role> Roles => Set<Role>();
    // public DbSet<List> Lists => Set<List>();
    // public DbSet<UserToList> UserToLists => Set<UserToList>();
    // public DbSet<Category> Categories => Set<Category>();
    // public DbSet<ListItem> ListItems => Set<ListItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically set CreatedAt for new entities
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

