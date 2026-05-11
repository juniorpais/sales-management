using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, Result<CreateSaleResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<CreateSaleResult>> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleValidator();
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var existing = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existing is not null)
            return Result.Fail<CreateSaleResult>($"Sale with number '{command.SaleNumber}' already exists.");

        var sale = Sale.Create(command.SaleNumber, command.Date, command.CustomerId, command.CustomerName, command.BranchId, command.BranchName);

        foreach (var item in command.Items)
        {
            var itemResult = sale.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
            if (itemResult.IsFailed)
                return Result.Fail<CreateSaleResult>(itemResult.Errors);
        }

        var created = await _saleRepository.CreateAsync(sale, cancellationToken);

        foreach (var domainEvent in created.DomainEvents)
            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);

        created.ClearDomainEvents();

        return Result.Ok(_mapper.Map<CreateSaleResult>(created));
    }
}
