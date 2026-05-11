using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Common.Events;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCreatedEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public string SaleNumber { get; }
    public DateTime OccurredAt { get; }

    public SaleCreatedEvent(Guid saleId, string saleNumber)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        OccurredAt = DateTime.UtcNow;
    }
}
