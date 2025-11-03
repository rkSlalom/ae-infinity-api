namespace AeInfinity.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

