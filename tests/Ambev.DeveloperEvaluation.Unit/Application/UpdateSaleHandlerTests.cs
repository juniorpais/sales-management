using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher);
    }

    [Fact(DisplayName = "Given valid command When updating sale Then returns success result")]
    public async Task Given_ValidCommand_When_UpdatingSale_Then_ReturnsSuccessResult()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(sale.Id);

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>())
            .Returns(new UpdateSaleResult
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                CustomerName = command.CustomerName,
                BranchName = command.BranchName
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing sale When updating Then returns failure")]
    public async Task Given_NonExistingSale_When_Updating_Then_ReturnsFailure()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("not found"));
    }

    [Fact(DisplayName = "Given cancelled sale When updating Then returns failure")]
    public async Task Given_CancelledSale_When_Updating_Then_ReturnsFailure()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(sale.Id);

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("cancelled"));
    }

    [Fact(DisplayName = "Given valid sale When updated Then publishes SaleModifiedEvent")]
    public async Task Given_ValidSale_When_Updated_Then_PublishesSaleModifiedEvent()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(sale.Id);

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>())
            .Returns(new UpdateSaleResult { Id = sale.Id });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _eventPublisher.Received().PublishAsync(
            Arg.Any<SaleModifiedEvent>(),
            Arg.Any<CancellationToken>());
    }
}
