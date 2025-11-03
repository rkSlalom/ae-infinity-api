using FluentValidation;

namespace AeInfinity.Application.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .WithMessage("Display name is required.")
            .MinimumLength(2)
            .WithMessage("Display name must be at least 2 characters.")
            .MaximumLength(100)
            .WithMessage("Display name must not exceed 100 characters.");

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(500)
            .WithMessage("Avatar URL must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.AvatarUrl));
    }
}
