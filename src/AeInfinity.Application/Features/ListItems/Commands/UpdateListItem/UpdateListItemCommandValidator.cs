using FluentValidation;

namespace AeInfinity.Application.Features.ListItems.Commands.UpdateListItem;

public class UpdateListItemCommandValidator : AbstractValidator<UpdateListItemCommand>
{
    public UpdateListItemCommandValidator()
    {
        RuleFor(x => x.ListId)
            .NotEmpty()
            .WithMessage("List ID is required.");

        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithMessage("Item ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Item name is required.")
            .MaximumLength(200)
            .WithMessage("Item name must not exceed 200 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.Unit)
            .MaximumLength(50)
            .WithMessage("Unit must not exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.Unit));

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes must not exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(2048)
            .WithMessage("Image URL must not exceed 2048 characters.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.Position)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Position must be greater than or equal to 0.");
    }
}

