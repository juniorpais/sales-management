using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Common.Events;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class ItemCancelledEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public Guid ProductId { get; }
    public DateTime OccurredAt { get; }

    public ItemCancelledEvent(Guid saleId, Guid productId)
    {
        SaleId = saleId;
        ProductId = productId;
        OccurredAt = DateTime.UtcNow;
    }
}
