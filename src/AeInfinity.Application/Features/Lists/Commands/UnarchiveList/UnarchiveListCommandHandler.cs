using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Commands.UnarchiveList;

public class UnarchiveListCommandHandler : IRequestHandler<UnarchiveListCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;
    private readonly IRealtimeNotificationService _realtimeService;

    public UnarchiveListCommandHandler(
        IApplicationDbContext context, 
        IListPermissionService permissionService,
        IRealtimeNotificationService realtimeService)
    {
        _context = context;
        _permissionService = permissionService;
        _realtimeService = realtimeService;
    }

    public async Task<Unit> Handle(UnarchiveListCommand request, CancellationToken cancellationToken)
    {
        // Check if user can unarchive this list (owner only)
        var canUnarchive = await _permissionService.CanUserArchiveListAsync(request.UserId, request.ListId, cancellationToken);
        if (!canUnarchive)
        {
            throw new ForbiddenException("Only the list owner can unarchive the list.");
        }

        var list = await _context.Lists
            .FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);

        if (list == null)
        {
            throw new NotFoundException("List", request.ListId);
        }

        if (!list.IsArchived)
        {
            throw new ValidationException("List is not archived.");
        }

        list.IsArchived = false;
        list.ArchivedAt = null;
        list.ArchivedBy = null;

        await _context.SaveChangesAsync(cancellationToken);

        // Broadcast list unarchived event to SignalR clients
        await _realtimeService.NotifyListArchivedAsync(list.Id, false);

        return Unit.Value;
    }
}

