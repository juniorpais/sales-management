using FluentResults;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public class CreateCartCommand : IRequest<Result<CreateCartResult>>
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public List<CreateCartItemCommand> Items { get; set; } = [];
}

public class CreateCartItemCommand
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
