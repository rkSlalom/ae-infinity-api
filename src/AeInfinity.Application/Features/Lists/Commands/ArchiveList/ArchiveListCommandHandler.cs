using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Commands.ArchiveList;

public class ArchiveListCommandHandler : IRequestHandler<ArchiveListCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;
    private readonly IRealtimeNotificationService _realtimeService;

    public ArchiveListCommandHandler(
        IApplicationDbContext context, 
        IListPermissionService permissionService,
        IRealtimeNotificationService realtimeService)
    {
        _context = context;
        _permissionService = permissionService;
        _realtimeService = realtimeService;
    }

    public async Task<Unit> Handle(ArchiveListCommand request, CancellationToken cancellationToken)
    {
        // Check if user can archive this list (owner only)
        var canArchive = await _permissionService.CanUserArchiveListAsync(request.UserId, request.ListId, cancellationToken);
        if (!canArchive)
        {
            throw new ForbiddenException("Only the list owner can archive the list.");
        }

        var list = await _context.Lists
            .FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);

        if (list == null)
        {
            throw new NotFoundException("List", request.ListId);
        }

        if (list.IsArchived)
        {
            throw new ValidationException("List is already archived.");
        }

        list.IsArchived = true;
        list.ArchivedAt = DateTime.UtcNow;
        list.ArchivedBy = request.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        // Broadcast list archived event to SignalR clients
        await _realtimeService.NotifyListArchivedAsync(list.Id, true);

        return Unit.Value;
    }
}

