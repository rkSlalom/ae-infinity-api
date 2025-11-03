using FluentValidation;

namespace AeInfinity.Application.Features.Lists.Commands.ChangeCollaboratorRole;

public class ChangeCollaboratorRoleCommandValidator : AbstractValidator<ChangeCollaboratorRoleCommand>
{
    public ChangeCollaboratorRoleCommandValidator()
    {
        RuleFor(x => x.ListId)
            .NotEmpty()
            .WithMessage("List ID is required.");

        RuleFor(x => x.CollaboratorUserId)
            .NotEmpty()
            .WithMessage("Collaborator user ID is required.");

        RuleFor(x => x.NewRoleId)
            .NotEmpty()
            .WithMessage("New role ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}

