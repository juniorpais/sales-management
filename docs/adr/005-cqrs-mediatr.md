# ADR 005 — CQRS with MediatR

## Status
Accepted

## Context
The application needs to handle both read and write operations across multiple resources (Sales, Products, Carts, Users). A traditional service layer would grow into a large, coupled class over time.

## Decision
We adopt **CQRS** (Command Query Responsibility Segregation) via **MediatR**. Each operation is a self-contained request/handler pair:

- Commands mutate state: `CreateSaleCommand`, `UpdateSaleCommand`, `CancelSaleCommand`
- Queries read state: `GetSaleQuery`, `GetSalesQuery`

MediatR pipeline behaviors handle cross-cutting concerns:
- `ValidationBehavior<TRequest, TResponse>` — runs FluentValidation before every handler

## Consequences
**Positive:**
- Each handler has a single responsibility — easy to test and reason about
- New operations are added without modifying existing code (Open/Closed Principle)
- Pipeline behaviors apply cross-cutting concerns uniformly (validation, logging)
- Commands and queries are clearly separated — different scaling and caching strategies possible

**Negative:**
- More files per feature compared to a simple service class
- Slight indirection — requires understanding the MediatR pipeline to debug
