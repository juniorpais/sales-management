# ADR 004 — Domain Events Collected in Entity, Published After Persist

## Status
Accepted

## Context
The challenge requires publishing events (SaleCreated, SaleModified, SaleCancelled, ItemCancelled) when domain operations occur. Two approaches: publish immediately inside the entity, or collect and publish after persistence.

## Decision
Domain events are **collected inside the aggregate** via `AddDomainEvent()` and **published by the handler after `SaveChanges`** via `IEventPublisher`.

```csharp
// Entity collects the event
public Result Cancel()
{
    IsCancelled = true;
    AddDomainEvent(new SaleCancelledEvent(Id));
    return Result.Ok();
}

// Handler publishes after persist
await _saleRepository.UpdateAsync(sale, cancellationToken);
foreach (var domainEvent in sale.DomainEvents)
    await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
sale.ClearDomainEvents();
```

## Consequences
**Positive:**
- Events are only published if the database write succeeds — no inconsistency between state and events
- Entities remain infrastructure-agnostic (no dependency on `IEventPublisher`)
- Easy to test: assert `sale.DomainEvents.Count` without needing a real publisher

**Negative:**
- If the publisher call fails after a successful save, events are lost (mitigated by Polly retry with 3 attempts)
