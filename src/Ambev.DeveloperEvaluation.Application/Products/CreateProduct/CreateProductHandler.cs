using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResult>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CreateProductHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<CreateProductResult>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateProductValidator();
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var rating = new Rating(command.RatingRate, command.RatingCount);
        var productResult = Product.Create(command.Title, command.Price, command.Description, command.Category, command.Image, rating);
        if (productResult.IsFailed)
            return Result.Fail<CreateProductResult>(productResult.Errors);

        var created = await _productRepository.CreateAsync(productResult.Value, cancellationToken);
        return Result.Ok(_mapper.Map<CreateProductResult>(created));
    }
}
