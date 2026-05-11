using AutoMapper;
using FluentResults;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct;

public class GetProductHandler : IRequestHandler<GetProductQuery, Result<GetProductResult>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetProductResult>> Handle(GetProductQuery query, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(query.Id, cancellationToken);
        if (product is null)
            return Result.Fail<GetProductResult>($"Product with ID '{query.Id}' not found.");

        return Result.Ok(_mapper.Map<GetProductResult>(product));
    }
}
