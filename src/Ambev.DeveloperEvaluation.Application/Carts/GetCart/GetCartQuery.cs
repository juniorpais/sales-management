using FluentResults;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

public class GetCartQuery : IRequest<Result<GetCartResult>>
{
    public Guid Id { get; set; }
}
