using FluentResults;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleQuery : IRequest<Result<GetSaleResult>>
{
    public Guid Id { get; set; }
}
