using FluentResults;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
