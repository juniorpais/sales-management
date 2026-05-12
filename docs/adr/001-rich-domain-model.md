# ADR 001 — Rich Domain Model over Anemic Domain

## Status
Accepted

## Context
The challenge requires implementing business rules for quantity-based discounts and sale cancellation. There are two common approaches: anemic domain (entities are data bags, logic lives in services/handlers) or rich domain (entities own their behavior and invariants).

## Decision
We adopted a **rich domain model**. Business rules live exclusively inside the domain entities:
- `SaleItem.CalculateDiscount()` — discount tiers enforced in the entity constructor
- `Sale.Cancel()` — cancellation logic with event dispatch inside the aggregate
- `Sale.CancelItem()` — item-level cancellation enforced by the aggregate root

## Consequences
**Positive:**
- Business rules are co-located with the data they protect — impossible to bypass
- Domain is self-validating; handlers become thin orchestrators
- Easier to test business logic in isolation (pure unit tests, no mocks needed for rules)

**Negative:**
- Entities require private setters and controlled constructors — more boilerplate than simple POCOs
- EF Core requires additional configuration to reflect private members
