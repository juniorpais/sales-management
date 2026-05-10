using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class CartValidator : AbstractValidator<Cart>
{
    public CartValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(c => c.UserName)
            .NotEmpty().WithMessage("User name is required.")
            .MaximumLength(200).WithMessage("User name cannot exceed 200 characters.");

        RuleFor(c => c.Date)
            .NotEmpty().WithMessage("Date is required.");
    }
}
