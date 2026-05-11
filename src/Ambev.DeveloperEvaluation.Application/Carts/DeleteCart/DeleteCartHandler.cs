using FluentResults;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

public class DeleteCartHandler : IRequestHandler<DeleteCartCommand, Result>
{
    private readonly ICartRepository _cartRepository;

    public DeleteCartHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<Result> Handle(DeleteCartCommand command, CancellationToken cancellationToken)
    {
        var deleted = await _cartRepository.DeleteAsync(command.Id, cancellationToken);
        return deleted ? Result.Ok() : Result.Fail($"Cart with ID '{command.Id}' not found.");
    }
}
