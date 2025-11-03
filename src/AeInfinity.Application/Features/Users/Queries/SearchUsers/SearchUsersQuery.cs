using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Users.Queries.SearchUsers;

public class SearchUsersQuery : IRequest<List<UserBasicDto>>
{
    public string Query { get; set; } = string.Empty;
}
