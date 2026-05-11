using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Common.Events;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCancelledEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public DateTime OccurredAt { get; }

    public SaleCancelledEvent(Guid saleId)
    {
        SaleId = saleId;
        OccurredAt = DateTime.UtcNow;
    }
}
