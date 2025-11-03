using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Queries.GetLists;

public class GetListsQueryHandler : IRequestHandler<GetListsQuery, List<ListDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetListsQueryHandler(IApplicationDbContext context, IMapper _mapper)
    {
        _context = context;
        this._mapper = _mapper;
    }

    public async Task<List<ListDto>> Handle(GetListsQuery request, CancellationToken cancellationToken)
    {
        // Get lists where user is owner or collaborator
        var query = _context.Lists
            .Include(l => l.Items)
            .Include(l => l.Collaborators)
            .AsNoTracking()
            .Where(l => l.OwnerId == request.UserId || 
                       l.Collaborators.Any(c => c.UserId == request.UserId && !c.IsPending));

        // Filter archived lists if needed
        if (!request.IncludeArchived)
        {
            query = query.Where(l => !l.IsArchived);
        }

        var lists = await query
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ListDto>>(lists);
    }
}

