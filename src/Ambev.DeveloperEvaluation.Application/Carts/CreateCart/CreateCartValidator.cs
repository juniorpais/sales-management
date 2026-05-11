using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public class CreateCartValidator : AbstractValidator<CreateCartCommand>
{
    public CreateCartValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Date).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.ProductName).NotEmpty().MaximumLength(200);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}
