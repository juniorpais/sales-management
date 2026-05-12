# ADR 003 — External Identities Pattern for Cross-Domain References

## Status
Accepted

## Context
The Sales bounded context needs to reference entities from other domains: Customer, Branch, and Product. In a monolith it's tempting to use foreign keys and join tables. In DDD, this creates tight coupling between bounded contexts.

## Decision
We use the **External Identities** pattern: reference external entities by storing their `Id` alongside a **denormalized snapshot of their display name** at the time of the transaction.

```csharp
public class Sale : BaseEntity
{
    public Guid CustomerId { get; private set; }      // reference
    public string CustomerName { get; private set; }  // denormalized snapshot
    public Guid BranchId { get; private set; }
    public string BranchName { get; private set; }
}
```

## Consequences
**Positive:**
- Sales context is fully autonomous — no joins across bounded contexts
- Historical accuracy: sale records preserve the customer/branch name at the time of sale, even if the original entity is later renamed or deleted
- Bounded contexts can evolve independently without breaking each other

**Negative:**
- Data duplication — names are stored redundantly
- If a name correction is needed historically, it requires explicit update logic
