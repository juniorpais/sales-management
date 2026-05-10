using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Events;
using FluentResults;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public string SaleNumber { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }

    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;

    public Guid BranchId { get; private set; }
    public string BranchName { get; private set; } = string.Empty;

    public bool IsCancelled { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<SaleItem> _items = [];
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);

    private Sale() { }

    public static Sale Create(
        string saleNumber,
        DateTime date,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName)
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = saleNumber,
            Date = date,
            CustomerId = customerId,
            CustomerName = customerName,
            BranchId = branchId,
            BranchName = branchName,
            IsCancelled = false,
            CreatedAt = DateTime.UtcNow
        };

        sale.AddDomainEvent(new SaleCreatedEvent(sale.Id, sale.SaleNumber));
        return sale;
    }

    public Result AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (IsCancelled)
            return Result.Fail("Cannot add items to a cancelled sale.");

        var existing = _items.FirstOrDefault(i => i.ProductId == productId && !i.IsCancelled);
        if (existing is not null)
        {
            var updateResult = existing.UpdateQuantity(existing.Quantity + quantity);
            if (updateResult.IsFailed) return updateResult;

            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new SaleModifiedEvent(Id));
            return Result.Ok();
        }

        var itemResult = SaleItem.Create(Id, productId, productName, quantity, unitPrice);
        if (itemResult.IsFailed)
            return itemResult.ToResult();

        _items.Add(itemResult.Value);
        UpdatedAt = DateTime.UtcNow;
        return Result.Ok();
    }

    public Result Update(DateTime date, Guid customerId, string customerName, Guid branchId, string branchName)
    {
        if (IsCancelled)
            return Result.Fail("Cannot update a cancelled sale.");

        Date = date;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SaleModifiedEvent(Id));
        return Result.Ok();
    }

    public Result Cancel()
    {
        if (IsCancelled)
            return Result.Fail("Sale is already cancelled.");

        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SaleCancelledEvent(Id));
        return Result.Ok();
    }

    public Result CancelItem(Guid productId)
    {
        if (IsCancelled)
            return Result.Fail("Cannot cancel items of an already cancelled sale.");

        var item = _items.FirstOrDefault(i => i.ProductId == productId && !i.IsCancelled);
        if (item is null)
            return Result.Fail("Item not found or already cancelled.");

        item.Cancel();
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ItemCancelledEvent(Id, productId));
        return Result.Ok();
    }
}
