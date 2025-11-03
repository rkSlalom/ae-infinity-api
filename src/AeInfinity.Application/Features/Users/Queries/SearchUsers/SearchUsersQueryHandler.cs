using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Users.Queries.SearchUsers;

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, List<UserBasicDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SearchUsersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<UserBasicDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var normalizedQuery = request.Query.ToLowerInvariant();

        var users = await _context.Users
            .AsNoTracking()
            .Where(u => u.EmailNormalized.Contains(normalizedQuery) || 
                       u.DisplayName.ToLower().Contains(normalizedQuery))
            .Take(10) // Limit results to 10
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<UserBasicDto>>(users);
    }
}
