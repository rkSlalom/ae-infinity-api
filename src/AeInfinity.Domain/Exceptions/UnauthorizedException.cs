namespace AeInfinity.Domain.Exceptions;

/// <summary>
/// Exception thrown when authentication is required but not provided
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException() : base("Authentication is required to access this resource.")
    {
    }

    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

