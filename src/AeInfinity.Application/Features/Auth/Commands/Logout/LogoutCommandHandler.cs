using MediatR;

namespace AeInfinity.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    public Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // In a production application, we would:
        // 1. Add token to blacklist/revocation list
        // 2. Store in Redis or database
        // 3. Check blacklist on each authenticated request
        
        // For this prototype with in-memory database, logout is handled client-side
        // Client should delete the JWT token from storage
        
        return Task.FromResult(Unit.Value);
    }
}

