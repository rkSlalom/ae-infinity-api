using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Users.Queries.GetCurrentUser;

public class GetCurrentUserQuery : IRequest<UserDto>
{
    public Guid UserId { get; set; }
}

