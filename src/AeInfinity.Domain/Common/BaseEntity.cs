namespace AeInfinity.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Soft Delete Fields
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
}

