using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Users.Commands.DeleteUserAccount;

public class DeleteUserAccountCommandHandler : IRequestHandler<DeleteUserAccountCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteUserAccountCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteUserAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User", request.UserId);
        }

        // Soft delete the user
        // EF Core will handle setting IsDeleted, DeletedAt, and DeletedBy through SaveChangesAsync override
        _context.Users.Remove(user);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
