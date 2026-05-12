using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Given valid data When creating sale Then raises SaleCreatedEvent")]
    public void Given_ValidData_When_CreatingSale_Then_RaisesSaleCreatedEvent()
    {
        var sale = SaleTestData.GenerateValidSale();

        sale.DomainEvents.Should().ContainSingle(e => e is SaleCreatedEvent);
    }

    [Fact(DisplayName = "Given sale When adding valid item Then item is added and total is calculated")]
    public void Given_Sale_When_AddingValidItem_Then_ItemAddedAndTotalCalculated()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.ClearDomainEvents();
        var (productId, productName, _, unitPrice) = SaleTestData.GenerateValidItem(3);

        var result = sale.AddItem(productId, productName, 3, unitPrice);

        result.IsSuccess.Should().BeTrue();
        sale.Items.Should().HaveCount(1);
        sale.TotalAmount.Should().Be(3 * unitPrice);
    }

    [Fact(DisplayName = "Given sale When adding item with quantity above 20 Then returns failure")]
    public void Given_Sale_When_AddingItemAbove20_Then_ReturnsFailure()
    {
        var sale = SaleTestData.GenerateValidSale();
        var (productId, productName, _, unitPrice) = SaleTestData.GenerateValidItem();

        var result = sale.AddItem(productId, productName, 21, unitPrice);

        result.IsFailed.Should().BeTrue();
        sale.Items.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given active sale When cancelling Then IsCancelled is true and raises event")]
    public void Given_ActiveSale_When_Cancelling_Then_IsCancelledAndRaisesEvent()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.ClearDomainEvents();

        var result = sale.Cancel();

        result.IsSuccess.Should().BeTrue();
        sale.IsCancelled.Should().BeTrue();
        sale.DomainEvents.Should().ContainSingle(e => e is SaleCancelledEvent);
    }

    [Fact(DisplayName = "Given cancelled sale When cancelling again Then returns failure")]
    public void Given_CancelledSale_When_CancellingAgain_Then_ReturnsFailure()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        var result = sale.Cancel();

        result.IsFailed.Should().BeTrue();
    }

    [Fact(DisplayName = "Given sale with item When cancelling item Then item is cancelled and raises event")]
    public void Given_SaleWithItem_When_CancellingItem_Then_ItemCancelledAndRaisesEvent()
    {
        var sale = SaleTestData.GenerateValidSale();
        var productId = Guid.NewGuid();
        sale.AddItem(productId, "Product", 3, 100m);
        sale.ClearDomainEvents();

        var result = sale.CancelItem(productId);

        result.IsSuccess.Should().BeTrue();
        sale.Items.First().IsCancelled.Should().BeTrue();
        sale.DomainEvents.Should().ContainSingle(e => e is ItemCancelledEvent);
    }

    [Fact(DisplayName = "Given sale When cancelling non-existing item Then returns failure")]
    public void Given_Sale_When_CancellingNonExistingItem_Then_ReturnsFailure()
    {
        var sale = SaleTestData.GenerateValidSale();

        var result = sale.CancelItem(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Fact(DisplayName = "Given sale with cancelled items When calculating total Then cancelled items excluded")]
    public void Given_SaleWithCancelledItems_When_CalculatingTotal_Then_CancelledItemsExcluded()
    {
        var sale = SaleTestData.GenerateValidSale();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        sale.AddItem(productId1, "Product 1", 2, 100m);
        sale.AddItem(productId2, "Product 2", 3, 50m);
        sale.CancelItem(productId2);

        sale.TotalAmount.Should().Be(200m);
    }
}
