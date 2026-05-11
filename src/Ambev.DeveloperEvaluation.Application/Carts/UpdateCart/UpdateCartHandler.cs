using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartHandler : IRequestHandler<UpdateCartCommand, Result<UpdateCartResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public UpdateCartHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<Result<UpdateCartResult>> Handle(UpdateCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateCartValidator();
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var cart = await _cartRepository.GetByIdAsync(command.Id, cancellationToken);
        if (cart is null)
            return Result.Fail<UpdateCartResult>($"Cart with ID '{command.Id}' not found.");

        cart.Update(command.Date);

        foreach (var item in command.Items)
        {
            var addResult = cart.AddItem(item.ProductId, item.ProductName, item.Quantity);
            if (addResult.IsFailed)
                return Result.Fail<UpdateCartResult>(addResult.Errors);
        }

        var updated = await _cartRepository.UpdateAsync(cart, cancellationToken);
        return Result.Ok(_mapper.Map<UpdateCartResult>(updated));
    }
}
