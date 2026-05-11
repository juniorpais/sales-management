using FluentResults;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartCommand : IRequest<Result<UpdateCartResult>>
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public List<UpdateCartItemCommand> Items { get; set; } = [];
}

public class UpdateCartItemCommand
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
