using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public class CreateCartHandler : IRequestHandler<CreateCartCommand, Result<CreateCartResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public CreateCartHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<Result<CreateCartResult>> Handle(CreateCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateCartValidator();
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var cartResult = Cart.Create(command.UserId, command.UserName, command.Date);
        if (cartResult.IsFailed)
            return Result.Fail<CreateCartResult>(cartResult.Errors);

        var cart = cartResult.Value;
        foreach (var item in command.Items)
        {
            var addResult = cart.AddItem(item.ProductId, item.ProductName, item.Quantity);
            if (addResult.IsFailed)
                return Result.Fail<CreateCartResult>(addResult.Errors);
        }

        var created = await _cartRepository.CreateAsync(cart, cancellationToken);
        return Result.Ok(_mapper.Map<CreateCartResult>(created));
    }
}
