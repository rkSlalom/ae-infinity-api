using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Entities;
using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.CreateList;

public class CreateListCommandHandler : IRequestHandler<CreateListCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateListCommand request, CancellationToken cancellationToken)
    {
        var list = new List
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            OwnerId = request.UserId,
            CreatedBy = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Lists.Add(list);
        await _context.SaveChangesAsync(cancellationToken);

        return list.Id;
    }
}

