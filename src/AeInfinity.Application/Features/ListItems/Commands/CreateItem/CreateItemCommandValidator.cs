using FluentValidation;

namespace AeInfinity.Application.Features.ListItems.Commands.CreateItem;

public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required")
            .Length(1, 100).WithMessage("Item name must be between 1 and 100 characters");
        
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
        
        RuleFor(x => x.Unit)
            .MaximumLength(50).WithMessage("Unit must be 50 characters or less")
            .When(x => !string.IsNullOrEmpty(x.Unit));
        
        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must be 500 characters or less")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

