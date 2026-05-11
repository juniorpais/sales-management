using FluentResults;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCarts;

public class GetCartsQuery : IRequest<Result<GetCartsResult>>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
}
