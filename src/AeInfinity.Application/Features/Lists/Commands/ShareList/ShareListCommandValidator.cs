using FluentValidation;

namespace AeInfinity.Application.Features.Lists.Commands.ShareList;

public class ShareListCommandValidator : AbstractValidator<ShareListCommand>
{
    public ShareListCommandValidator()
    {
        RuleFor(x => x.ListId)
            .NotEmpty()
            .WithMessage("List ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.InviteeEmail)
            .NotEmpty()
            .WithMessage("Invitee email is required.")
            .EmailAddress()
            .WithMessage("Invalid email address.")
            .MaximumLength(255)
            .WithMessage("Email must not exceed 255 characters.");

        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required.");
    }
}

