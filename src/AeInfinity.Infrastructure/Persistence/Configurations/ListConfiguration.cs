using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeInfinity.Infrastructure.Persistence.Configurations;

public class ListConfiguration : IEntityTypeConfiguration<Domain.Entities.List>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.List> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Description)
            .HasMaxLength(1000);

        builder.Property(l => l.OwnerId)
            .IsRequired();

        builder.Property(l => l.IsArchived)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(l => l.OwnerId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(l => l.CreatedAt)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(l => l.IsArchived)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(l => l.IsDeleted);

        builder.HasIndex(l => l.Name)
            .HasFilter("[IsDeleted] = 0");

        // Query filter for soft delete
        builder.HasQueryFilter(l => !l.IsDeleted);

        // Relationships
        builder.HasOne(l => l.Owner)
            .WithMany(u => u.OwnedLists)
            .HasForeignKey(l => l.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.Items)
            .WithOne(i => i.List)
            .HasForeignKey(i => i.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.Collaborators)
            .WithOne(c => c.List)
            .HasForeignKey(c => c.ListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

