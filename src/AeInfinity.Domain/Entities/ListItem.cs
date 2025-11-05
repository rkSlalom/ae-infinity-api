namespace AeInfinity.Domain.Entities;

public class ListItem
{
    // Primary Key
    public Guid Id { get; set; }
    
    // Foreign Keys
    public Guid ListId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? PurchasedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // Core Properties
    public string Name { get; set; } = string.Empty; // Required, 1-100 chars
    public decimal Quantity { get; set; } = 1; // Positive number, default 1
    public string? Unit { get; set; } // Optional (e.g., "gallons", "pieces")
    public string? Notes { get; set; } // Optional, max 500 chars
    public string? ImageUrl { get; set; } // Optional (future feature)
    
    // Status & Ordering
    public bool IsPurchased { get; set; } = false;
    public int Position { get; set; } = 0; // Display order (0-based)
    
    // Soft Delete (Constitution compliant)
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } // Null if not deleted
    public Guid? DeletedById { get; set; } // User who deleted
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? PurchasedAt { get; set; } // Null if not purchased
    
    // Navigation Properties
    public List List { get; set; } = null!;
    public Category? Category { get; set; }
    public User? Creator { get; set; }
    public User? PurchasedByUser { get; set; }
    public User? UpdatedByUser { get; set; }
    public User? DeletedBy { get; set; }
}
