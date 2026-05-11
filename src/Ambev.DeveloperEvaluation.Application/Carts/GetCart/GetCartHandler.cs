using AutoMapper;
using FluentResults;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

public class GetCartHandler : IRequestHandler<GetCartQuery, Result<GetCartResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public GetCartHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetCartResult>> Handle(GetCartQuery query, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(query.Id, cancellationToken);
        if (cart is null)
            return Result.Fail<GetCartResult>($"Cart with ID '{query.Id}' not found.");

        return Result.Ok(_mapper.Map<GetCartResult>(cart));
    }
}
