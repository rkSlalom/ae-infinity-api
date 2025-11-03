using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserBasicDto>
{
    public Guid UserId { get; set; }
}
