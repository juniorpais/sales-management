using FluentResults;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, Result>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventPublisher _eventPublisher;

    public CancelSaleHandler(ISaleRepository saleRepository, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleValidator();
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale is null)
            return Result.Fail($"Sale with ID '{command.Id}' not found.");

        var cancelResult = sale.Cancel();
        if (cancelResult.IsFailed)
            return cancelResult;

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        foreach (var domainEvent in sale.DomainEvents)
            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);

        sale.ClearDomainEvents();

        return Result.Ok();
    }
}
