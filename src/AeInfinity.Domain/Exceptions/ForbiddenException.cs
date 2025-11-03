namespace AeInfinity.Domain.Exceptions;

/// <summary>
/// Exception thrown when user is authenticated but lacks permission for the requested operation
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException() : base("You do not have permission to perform this action.")
    {
    }

    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException(string resource, string action)
        : base($"You do not have permission to {action} this {resource}.")
    {
    }

    public ForbiddenException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

