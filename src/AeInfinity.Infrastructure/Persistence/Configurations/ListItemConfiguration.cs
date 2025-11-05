using AeInfinity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeInfinity.Infrastructure.Persistence.Configurations;

public class ListItemConfiguration : IEntityTypeConfiguration<ListItem>
{
    public void Configure(EntityTypeBuilder<ListItem> builder)
    {
        builder.ToTable("ListItems");
        
        builder.HasKey(i => i.Id);
        
        // Properties
        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(i => i.Quantity)
            .IsRequired()
            .HasDefaultValue(1)
            .HasColumnType("decimal(18,2)");
        
        builder.Property(i => i.Unit)
            .HasMaxLength(50);
        
        builder.Property(i => i.Notes)
            .HasMaxLength(500);
        
        builder.Property(i => i.IsPurchased)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(i => i.Position)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(i => i.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(i => i.CreatedAt)
            .IsRequired();
        
        builder.Property(i => i.UpdatedAt)
            .IsRequired();
        
        // Relationships
        builder.HasOne(i => i.List)
            .WithMany(l => l.Items)
            .HasForeignKey(i => i.ListId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(i => i.Category)
            .WithMany()
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(i => i.Creator)
            .WithMany()
            .HasForeignKey(i => i.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(i => i.PurchasedByUser)
            .WithMany()
            .HasForeignKey(i => i.PurchasedBy)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(i => i.UpdatedByUser)
            .WithMany()
            .HasForeignKey(i => i.UpdatedBy)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(i => i.DeletedBy)
            .WithMany()
            .HasForeignKey(i => i.DeletedById)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Indexes
        builder.HasIndex(i => i.ListId)
            .HasDatabaseName("IX_ListItems_ListId");
        
        builder.HasIndex(i => i.IsDeleted)
            .HasDatabaseName("IX_ListItems_IsDeleted");
        
        builder.HasIndex(i => new { i.ListId, i.CategoryId })
            .HasDatabaseName("IX_ListItems_ListId_CategoryId");
        
        builder.HasIndex(i => new { i.ListId, i.IsPurchased })
            .HasDatabaseName("IX_ListItems_ListId_IsPurchased");
        
        builder.HasIndex(i => new { i.ListId, i.Position })
            .HasDatabaseName("IX_ListItems_ListId_Position");
        
        builder.HasIndex(i => new { i.CreatedBy, i.Name })
            .HasDatabaseName("IX_ListItems_CreatedBy_Name");
        
        // Global query filter for soft delete
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}
