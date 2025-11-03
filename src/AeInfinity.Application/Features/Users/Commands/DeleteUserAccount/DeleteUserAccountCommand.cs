using MediatR;

namespace AeInfinity.Application.Features.Users.Commands.DeleteUserAccount;

public class DeleteUserAccountCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
}
