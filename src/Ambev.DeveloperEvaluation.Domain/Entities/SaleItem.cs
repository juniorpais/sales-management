using Ambev.DeveloperEvaluation.Domain.Common;
using FluentResults;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; private set; }

    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public bool IsCancelled { get; private set; }

    public decimal TotalAmount => UnitPrice * Quantity * (1 - Discount);

    private SaleItem() { }

    public static Result<SaleItem> Create(Guid saleId, Guid productId, string productName, int quantity, decimal unitPrice)
    {
        var errors = new List<string>();

        if (quantity <= 0) errors.Add("Quantity must be greater than zero.");
        if (quantity > 20) errors.Add("Cannot sell more than 20 identical items.");
        if (unitPrice <= 0) errors.Add("Unit price must be greater than zero.");
        if (string.IsNullOrWhiteSpace(productName)) errors.Add("Product name is required.");

        if (errors.Count > 0)
            return Result.Fail<SaleItem>(errors);

        var item = new SaleItem
        {
            Id = Guid.NewGuid(),
            SaleId = saleId,
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Discount = CalculateDiscount(quantity),
            IsCancelled = false
        };

        return Result.Ok(item);
    }

    public Result UpdateQuantity(int quantity)
    {
        if (quantity <= 0) return Result.Fail("Quantity must be greater than zero.");
        if (quantity > 20) return Result.Fail("Cannot sell more than 20 identical items.");

        Quantity = quantity;
        Discount = CalculateDiscount(quantity);
        return Result.Ok();
    }

    public void Cancel() => IsCancelled = true;

    private static decimal CalculateDiscount(int quantity) => quantity switch
    {
        >= 10 => 0.20m,
        >= 4  => 0.10m,
        _     => 0m
    };
}
