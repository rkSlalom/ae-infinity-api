using AeInfinity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeInfinity.Infrastructure.Persistence.Configurations;

public class UserToListConfiguration : IEntityTypeConfiguration<UserToList>
{
    public void Configure(EntityTypeBuilder<UserToList> builder)
    {
        builder.HasKey(utl => utl.Id);

        builder.Property(utl => utl.ListId)
            .IsRequired();

        builder.Property(utl => utl.UserId)
            .IsRequired();

        builder.Property(utl => utl.RoleId)
            .IsRequired();

        builder.Property(utl => utl.InvitedBy)
            .IsRequired();

        builder.Property(utl => utl.InvitedAt)
            .IsRequired();

        builder.Property(utl => utl.IsPending)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(utl => utl.ListId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(utl => utl.UserId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(utl => utl.IsPending)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(utl => new { utl.ListId, utl.UserId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(utl => utl.IsDeleted);

        // Query filter for soft delete
        builder.HasQueryFilter(utl => !utl.IsDeleted);

        // Relationships
        builder.HasOne(utl => utl.List)
            .WithMany(l => l.Collaborators)
            .HasForeignKey(utl => utl.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(utl => utl.User)
            .WithMany(u => u.ListCollaborations)
            .HasForeignKey(utl => utl.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(utl => utl.Role)
            .WithMany(r => r.UserToLists)
            .HasForeignKey(utl => utl.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(utl => utl.InvitedByUser)
            .WithMany()
            .HasForeignKey(utl => utl.InvitedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

