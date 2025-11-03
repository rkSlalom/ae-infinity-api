using MediatR;

namespace AeInfinity.Application.Features.Auth.Commands.Logout;

public class LogoutCommand : IRequest<Unit>
{
    // In a real application, we would pass the token here for blacklisting
    // For this prototype with in-memory database, logout is client-side only
}

