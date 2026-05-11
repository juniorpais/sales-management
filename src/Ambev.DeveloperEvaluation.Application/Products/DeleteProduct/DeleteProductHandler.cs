using FluentResults;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var deleted = await _productRepository.DeleteAsync(command.Id, cancellationToken);
        return deleted ? Result.Ok() : Result.Fail($"Product with ID '{command.Id}' not found.");
    }
}
