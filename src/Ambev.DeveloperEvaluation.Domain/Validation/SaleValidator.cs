using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(s => s.SaleNumber)
            .NotEmpty().WithMessage("Sale number is required.")
            .MaximumLength(50).WithMessage("Sale number cannot exceed 50 characters.");

        RuleFor(s => s.Date)
            .NotEmpty().WithMessage("Sale date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Sale date cannot be in the future.");

        RuleFor(s => s.CustomerId)
            .NotEmpty().WithMessage("Customer is required.");

        RuleFor(s => s.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(200).WithMessage("Customer name cannot exceed 200 characters.");

        RuleFor(s => s.BranchId)
            .NotEmpty().WithMessage("Branch is required.");

        RuleFor(s => s.BranchName)
            .NotEmpty().WithMessage("Branch name is required.")
            .MaximumLength(200).WithMessage("Branch name cannot exceed 200 characters.");

        RuleForEach(s => s.Items)
            .SetValidator(new SaleItemValidator());
    }
}
