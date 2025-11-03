using AeInfinity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeInfinity.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.CanCreateItems)
            .IsRequired();

        builder.Property(r => r.CanEditItems)
            .IsRequired();

        builder.Property(r => r.CanDeleteItems)
            .IsRequired();

        builder.Property(r => r.CanEditOwnItemsOnly)
            .IsRequired();

        builder.Property(r => r.CanMarkPurchased)
            .IsRequired();

        builder.Property(r => r.CanEditListDetails)
            .IsRequired();

        builder.Property(r => r.CanManageCollaborators)
            .IsRequired();

        builder.Property(r => r.CanDeleteList)
            .IsRequired();

        builder.Property(r => r.CanArchiveList)
            .IsRequired();

        builder.Property(r => r.PriorityOrder)
            .IsRequired();

        // Indexes
        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(r => r.IsDeleted);

        // Query filter for soft delete
        builder.HasQueryFilter(r => !r.IsDeleted);

        // Relationships
        builder.HasMany(r => r.UserToLists)
            .WithOne(utl => utl.Role)
            .HasForeignKey(utl => utl.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

