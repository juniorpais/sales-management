using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<UpdateProductResult>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateProductHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<UpdateProductResult>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateProductValidator();
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (product is null)
            return Result.Fail<UpdateProductResult>($"Product with ID '{command.Id}' not found.");

        var rating = new Rating(command.RatingRate, command.RatingCount);
        var updateResult = product.Update(command.Title, command.Price, command.Description, command.Category, command.Image, rating);
        if (updateResult.IsFailed)
            return Result.Fail<UpdateProductResult>(updateResult.Errors);

        var updated = await _productRepository.UpdateAsync(product, cancellationToken);
        return Result.Ok(_mapper.Map<UpdateProductResult>(updated));
    }
}
