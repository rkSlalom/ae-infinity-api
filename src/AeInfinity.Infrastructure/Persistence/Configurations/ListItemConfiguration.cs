using AeInfinity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeInfinity.Infrastructure.Persistence.Configurations;

public class ListItemConfiguration : IEntityTypeConfiguration<ListItem>
{
    public void Configure(EntityTypeBuilder<ListItem> builder)
    {
        builder.HasKey(li => li.Id);

        builder.Property(li => li.ListId)
            .IsRequired();

        builder.Property(li => li.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(li => li.Quantity)
            .IsRequired()
            .HasPrecision(10, 2)
            .HasDefaultValue(1.0m);

        builder.Property(li => li.Unit)
            .HasMaxLength(50);

        builder.Property(li => li.CategoryId)
            .IsRequired();

        builder.Property(li => li.Notes)
            .HasMaxLength(1000);

        builder.Property(li => li.ImageUrl)
            .HasMaxLength(500);

        builder.Property(li => li.IsPurchased)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(li => li.Position)
            .IsRequired()
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(li => li.ListId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(li => li.CategoryId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(li => li.IsPurchased)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(li => new { li.ListId, li.Position })
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(li => li.CreatedBy)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(li => li.IsDeleted);

        builder.HasIndex(li => li.Name)
            .HasFilter("[IsDeleted] = 0");

        // Query filter for soft delete
        builder.HasQueryFilter(li => !li.IsDeleted);

        // Relationships
        builder.HasOne(li => li.List)
            .WithMany(l => l.Items)
            .HasForeignKey(li => li.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(li => li.Category)
            .WithMany(c => c.ListItems)
            .HasForeignKey(li => li.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(li => li.PurchasedByUser)
            .WithMany()
            .HasForeignKey(li => li.PurchasedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(li => li.Creator)
            .WithMany(u => u.CreatedItems)
            .HasForeignKey(li => li.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

