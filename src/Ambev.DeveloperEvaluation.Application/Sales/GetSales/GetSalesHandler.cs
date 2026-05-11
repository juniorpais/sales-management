using AutoMapper;
using FluentResults;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesHandler : IRequestHandler<GetSalesQuery, Result<GetSalesResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetSalesResult>> Handle(GetSalesQuery query, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _saleRepository.GetAllAsync(query.Page, query.Size, query.Order, cancellationToken);

        var result = new GetSalesResult
        {
            Data = _mapper.Map<IEnumerable<GetSaleResult>>(items),
            TotalItems = totalCount,
            CurrentPage = query.Page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.Size)
        };

        return Result.Ok(result);
    }
}
