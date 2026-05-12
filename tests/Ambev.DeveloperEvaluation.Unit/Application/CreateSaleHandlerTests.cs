using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _eventPublisher);
    }

    [Fact(DisplayName = "Given valid command When creating sale Then returns success result")]
    public async Task Given_ValidCommand_When_CreatingSale_Then_ReturnsSuccessResult()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = Sale.Create(command.SaleNumber, command.Date, command.CustomerId, command.CustomerName, command.BranchId, command.BranchName);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>())
            .Returns(new CreateSaleResult { Id = sale.Id, SaleNumber = sale.SaleNumber });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given duplicate sale number When creating sale Then returns failure")]
    public async Task Given_DuplicateSaleNumber_When_CreatingSale_Then_ReturnsFailure()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = Sale.Create(command.SaleNumber, command.Date, command.CustomerId, command.CustomerName, command.BranchId, command.BranchName);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains(command.SaleNumber));
        await _saleRepository.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid command When creating sale Then throws validation exception")]
    public async Task Given_InvalidCommand_When_CreatingSale_Then_ThrowsValidationException()
    {
        // Arrange
        var command = new CreateSaleCommand();

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given valid sale When created Then publishes SaleCreatedEvent")]
    public async Task Given_ValidSale_When_Created_Then_PublishesSaleCreatedEvent()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = Sale.Create(command.SaleNumber, command.Date, command.CustomerId, command.CustomerName, command.BranchId, command.BranchName);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>())
            .Returns(new CreateSaleResult { Id = sale.Id });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _eventPublisher.Received().PublishAsync(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }
}
