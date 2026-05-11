using AutoMapper;
using FluentResults;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCarts;

public class GetCartsHandler : IRequestHandler<GetCartsQuery, Result<GetCartsResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public GetCartsHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetCartsResult>> Handle(GetCartsQuery query, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _cartRepository.GetAllAsync(query.Page, query.Size, query.Order, cancellationToken);

        return Result.Ok(new GetCartsResult
        {
            Data = _mapper.Map<IEnumerable<GetCartResult>>(items),
            TotalItems = totalCount,
            CurrentPage = query.Page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.Size)
        });
    }
}
