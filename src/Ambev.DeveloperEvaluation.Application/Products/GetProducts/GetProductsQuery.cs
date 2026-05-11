using FluentResults;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProducts;

public class GetProductsQuery : IRequest<Result<GetProductsResult>>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
    public string? Category { get; set; }
}
