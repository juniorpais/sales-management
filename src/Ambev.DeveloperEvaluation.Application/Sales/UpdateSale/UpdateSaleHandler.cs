using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, Result<UpdateSaleResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<UpdateSaleResult>> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale is null)
            return Result.Fail<UpdateSaleResult>($"Sale with ID '{command.Id}' not found.");

        var updateResult = sale.Update(command.Date, command.CustomerId, command.CustomerName, command.BranchId, command.BranchName);
        if (updateResult.IsFailed)
            return Result.Fail<UpdateSaleResult>(updateResult.Errors);

        var updated = await _saleRepository.UpdateAsync(sale, cancellationToken);

        foreach (var domainEvent in updated.DomainEvents)
            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);

        updated.ClearDomainEvents();

        return Result.Ok(_mapper.Map<UpdateSaleResult>(updated));
    }
}
