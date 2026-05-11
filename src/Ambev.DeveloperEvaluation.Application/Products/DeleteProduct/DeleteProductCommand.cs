using FluentResults;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;

public class DeleteProductCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
