using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Entities;
using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.CreateList;

public class CreateListCommandHandler : IRequestHandler<CreateListCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IRealtimeNotificationService _realtimeService;

    public CreateListCommandHandler(
        IApplicationDbContext context,
        IRealtimeNotificationService realtimeService)
    {
        _context = context;
        _realtimeService = realtimeService;
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

        // Broadcast list created event to SignalR clients
        await _realtimeService.NotifyListCreatedAsync(list.Id, new
        {
            Id = list.Id,
            Name = list.Name,
            Description = list.Description,
            OwnerId = list.OwnerId,
            CreatedAt = list.CreatedAt
        });

        return list.Id;
    }
}

