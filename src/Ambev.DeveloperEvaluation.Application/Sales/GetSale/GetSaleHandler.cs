using AutoMapper;
using FluentResults;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler : IRequestHandler<GetSaleQuery, Result<GetSaleResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetSaleResult>> Handle(GetSaleQuery query, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(query.Id, cancellationToken);
        if (sale is null)
            return Result.Fail<GetSaleResult>($"Sale with ID '{query.Id}' not found.");

        return Result.Ok(_mapper.Map<GetSaleResult>(sale));
    }
}
