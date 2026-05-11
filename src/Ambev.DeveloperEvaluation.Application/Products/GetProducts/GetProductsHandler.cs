using AutoMapper;
using FluentResults;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<GetProductsResult>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetProductsResult>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var (items, totalCount) = string.IsNullOrWhiteSpace(query.Category)
            ? await _productRepository.GetAllAsync(query.Page, query.Size, query.Order, cancellationToken)
            : await _productRepository.GetByCategoryAsync(query.Category, query.Page, query.Size, query.Order, cancellationToken);

        return Result.Ok(new GetProductsResult
        {
            Data = _mapper.Map<IEnumerable<GetProductResult>>(items),
            TotalItems = totalCount,
            CurrentPage = query.Page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.Size)
        });
    }
}
