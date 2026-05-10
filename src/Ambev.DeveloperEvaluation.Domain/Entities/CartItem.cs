using Ambev.DeveloperEvaluation.Domain.Common;
using FluentResults;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }

    private CartItem() { }

    public static Result<CartItem> Create(Guid cartId, Guid productId, string productName, int quantity)
    {
        var errors = new List<string>();

        if (quantity <= 0) errors.Add("Quantity must be greater than zero.");
        if (string.IsNullOrWhiteSpace(productName)) errors.Add("Product name is required.");

        if (errors.Count > 0)
            return Result.Fail<CartItem>(errors);

        return Result.Ok(new CartItem
        {
            Id = Guid.NewGuid(),
            CartId = cartId,
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity
        });
    }

    public Result UpdateQuantity(int quantity)
    {
        if (quantity <= 0) return Result.Fail("Quantity must be greater than zero.");

        Quantity = quantity;
        return Result.Ok();
    }
}
