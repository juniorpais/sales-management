using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _handler = new CancelSaleHandler(_saleRepository, _eventPublisher);
    }

    [Fact(DisplayName = "Given existing active sale When cancelling Then returns success")]
    public async Task Given_ActiveSale_When_Cancelling_Then_ReturnsSuccess()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        var command = new CancelSaleCommand { Id = sale.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing sale When cancelling Then returns failure")]
    public async Task Given_NonExistingSale_When_Cancelling_Then_ReturnsFailure()
    {
        // Arrange
        _saleRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var command = new CancelSaleCommand { Id = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("not found"));
    }

    [Fact(DisplayName = "Given already cancelled sale When cancelling again Then returns failure")]
    public async Task Given_CancelledSale_When_CancellingAgain_Then_ReturnsFailure()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var command = new CancelSaleCommand { Id = sale.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact(DisplayName = "Given active sale When cancelling Then publishes SaleCancelledEvent")]
    public async Task Given_ActiveSale_When_Cancelling_Then_PublishesSaleCancelledEvent()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        var command = new CancelSaleCommand { Id = sale.Id };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _eventPublisher.Received().PublishAsync(
            Arg.Any<SaleCancelledEvent>(),
            Arg.Any<CancellationToken>());
    }
}
