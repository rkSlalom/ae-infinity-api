using AeInfinity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeInfinity.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Icon)
            .HasMaxLength(50);

        builder.Property(c => c.Color)
            .HasMaxLength(7); // #RRGGBB format

        builder.Property(c => c.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.IsCustom)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.SortOrder)
            .IsRequired()
            .HasDefaultValue(999);

        // Indexes
        builder.HasIndex(c => c.IsDefault)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(c => c.CustomOwnerId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(c => c.SortOrder)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(c => c.IsDeleted);

        // Query filter for soft delete
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Relationships
        builder.HasOne(c => c.CustomOwner)
            .WithMany(u => u.CustomCategories)
            .HasForeignKey(c => c.CustomOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.ListItems)
            .WithOne(i => i.Category)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

