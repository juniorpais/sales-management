using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleModifiedEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public DateTime OccurredAt { get; }

    public SaleModifiedEvent(Guid saleId)
    {
        SaleId = saleId;
        OccurredAt = DateTime.UtcNow;
    }
}
