using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    [Fact(DisplayName = "Given quantity below 4 When creating item Then no discount applied")]
    public void Given_QuantityBelow4_When_CreatingItem_Then_NoDiscount()
    {
        var result = SaleItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Product", 3, 100m);

        result.IsSuccess.Should().BeTrue();
        result.Value.Discount.Should().Be(0m);
        result.Value.TotalAmount.Should().Be(300m);
    }

    [Theory(DisplayName = "Given quantity between 4 and 9 When creating item Then 10% discount applied")]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(9)]
    public void Given_QuantityBetween4And9_When_CreatingItem_Then_10PercentDiscount(int quantity)
    {
        var result = SaleItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Product", quantity, 100m);

        result.IsSuccess.Should().BeTrue();
        result.Value.Discount.Should().Be(0.10m);
        result.Value.TotalAmount.Should().Be(quantity * 100m * 0.90m);
    }

    [Theory(DisplayName = "Given quantity between 10 and 20 When creating item Then 20% discount applied")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void Given_QuantityBetween10And20_When_CreatingItem_Then_20PercentDiscount(int quantity)
    {
        var result = SaleItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Product", quantity, 100m);

        result.IsSuccess.Should().BeTrue();
        result.Value.Discount.Should().Be(0.20m);
        result.Value.TotalAmount.Should().Be(quantity * 100m * 0.80m);
    }

    [Fact(DisplayName = "Given quantity above 20 When creating item Then returns failure")]
    public void Given_QuantityAbove20_When_CreatingItem_Then_ReturnsFailure()
    {
        var result = SaleItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Product", 21, 100m);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("20"));
    }

    [Fact(DisplayName = "Given quantity zero When creating item Then returns failure")]
    public void Given_QuantityZero_When_CreatingItem_Then_ReturnsFailure()
    {
        var result = SaleItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Product", 0, 100m);

        result.IsFailed.Should().BeTrue();
    }

    [Fact(DisplayName = "Given negative unit price When creating item Then returns failure")]
    public void Given_NegativeUnitPrice_When_CreatingItem_Then_ReturnsFailure()
    {
        var result = SaleItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Product", 2, -10m);

        result.IsFailed.Should().BeTrue();
    }

    [Fact(DisplayName = "Given valid item When updating quantity to above 20 Then returns failure")]
    public void Given_ValidItem_When_UpdatingQuantityAbove20_Then_ReturnsFailure()
    {
        var item = SaleItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Product", 5, 100m).Value;

        var result = item.UpdateQuantity(21);

        result.IsFailed.Should().BeTrue();
    }

    [Fact(DisplayName = "Given item with 9 items When updating to 10 Then discount changes to 20 percent")]
    public void Given_ItemWith9Items_When_UpdatingTo10_Then_DiscountChangesTo20Percent()
    {
        var item = SaleItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Product", 9, 100m).Value;
        item.Discount.Should().Be(0.10m);

        item.UpdateQuantity(10);

        item.Discount.Should().Be(0.20m);
    }
}
