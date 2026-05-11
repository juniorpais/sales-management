using FluentResults;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.GetCategories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<string>>>
{
    private readonly IProductRepository _productRepository;

    public GetCategoriesHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<IEnumerable<string>>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        var categories = await _productRepository.GetCategoriesAsync(cancellationToken);
        return Result.Ok(categories);
    }
}
