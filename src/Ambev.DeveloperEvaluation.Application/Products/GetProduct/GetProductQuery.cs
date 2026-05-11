using FluentResults;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct;

public class GetProductQuery : IRequest<Result<GetProductResult>>
{
    public Guid Id { get; set; }
}
