using Ambev.DeveloperEvaluation.Domain.Common;
using FluentResults;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart() { }

    public static Result<Cart> Create(Guid userId, string userName, DateTime date)
    {
        var errors = new List<string>();

        if (userId == Guid.Empty) errors.Add("UserId is required.");
        if (string.IsNullOrWhiteSpace(userName)) errors.Add("User name is required.");

        if (errors.Count > 0)
            return Result.Fail<Cart>(errors);

        return Result.Ok(new Cart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserName = userName,
            Date = date,
            CreatedAt = DateTime.UtcNow
        });
    }

    public Result AddItem(Guid productId, string productName, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
            return existing.UpdateQuantity(quantity);

        var itemResult = CartItem.Create(Id, productId, productName, quantity);
        if (itemResult.IsFailed) return itemResult.ToResult();

        _items.Add(itemResult.Value);
        UpdatedAt = DateTime.UtcNow;
        return Result.Ok();
    }

    public Result RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
            return Result.Fail("Item not found in cart.");

        _items.Remove(item);
        UpdatedAt = DateTime.UtcNow;
        return Result.Ok();
    }

    public Result Update(DateTime date)
    {
        Date = date;
        UpdatedAt = DateTime.UtcNow;
        return Result.Ok();
    }
}
