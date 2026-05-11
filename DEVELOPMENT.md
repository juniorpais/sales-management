# Development Plan — Sales Management API

## Progress Overview

| Phase | Description | Branch | Status | Progress |
|-------|-------------|--------|--------|----------|
| 1 | Project Setup | `main` | ✅ Done | 100% |
| 2 | Database + ORM Mapping | `feature/database-orm` | ✅ Done* | 100% |
| 3 | Data Manipulation Layer | `feature/data-manipulation` | ✅ Done* | 100% |
| 4 | Services Layer (Rebus) | `feature/services-layer` | ⬜ Pending | 0% |
| 5 | Business Rules | `feature/business-rules` | ✅ Done* | 100% |
| 6 | Application Log | `feature/app-logging` | ⬜ Pending | 0% |
| 7 | Tests | `feature/tests` | ⬜ Pending | 0% |
| 8 | Documentation | `feature/documentation` | ⬜ Pending | 0% |

> *Phases 2, 3 and 5 were developed together under `feature/domain-layer` and `feature/application-layer` following a DDD-first approach. The branch names above reflect the commit segregation convention from the challenge manual.

**Overall Progress: 55% (44 / 80 tasks completed)**

---

## Git Flow

```
main
 └── develop
      ├── feature/database-orm
      ├── feature/data-manipulation
      ├── feature/services-layer
      ├── feature/business-rules
      ├── feature/app-logging
      ├── feature/tests
      └── feature/documentation
```

> Each `feature/*` branches from `develop` and merges back via PR.
> `develop` merges into `main` via `release/x.y.z` when ready.
> Hotfixes branch from `main` → `hotfix/description` → merge into `main` + `develop`.

---

## Semantic Commit Convention

```
feat:     new feature
fix:      bug fix
chore:    build, tooling, config
refactor: code change without new feature
test:     adding or fixing tests
docs:     documentation only
style:    formatting, no logic change
```

---

## Phase 1 — Project Setup `main`

**Progress: 100% (4/4)**

- [x] Move template structure to repository root (`src/`, `tests/`, `docs/`)
- [x] Validate solution file and Docker Compose baseline
- [x] `.editorconfig`, `.dockerignore`, `.gitignore` in place
- [x] Development plan documented

---

## Phase 2 — Database + ORM Mapping `feature/database-orm`

**Progress: 100% (10/10)**

> EF Core + PostgreSQL. Fluent API mapping. AutoMapper profiles.
> MongoDB considered for document storage (logs/events).

- [x] `Sale` EF Core configuration (table, columns, owned collections)
- [x] `SaleItem` EF Core configuration
- [x] `Product` EF Core configuration with owned `Rating`
- [x] `Cart` EF Core configuration
- [x] `CartItem` EF Core configuration
- [x] Register all configurations in `DefaultContext`
- [x] `SaleRepository` with pagination + filtering + ordering
- [x] `ProductRepository`
- [x] `CartRepository`
- [x] EF Core migration for all new tables

---

## Phase 3 — Data Manipulation Layer `feature/data-manipulation`

**Progress: 100% (20/20)**

> MediatR (CQRS). AutoMapper profiles. FluentValidation. FluentResults.

### Sales
- [x] `CreateSaleCommand` + `CreateSaleHandler`
- [x] `UpdateSaleCommand` + `UpdateSaleHandler`
- [x] `CancelSaleCommand` + `CancelSaleHandler`
- [x] `GetSaleQuery` + `GetSaleHandler`
- [x] `GetSalesQuery` + `GetSalesHandler` (paginated)

### Products
- [x] `CreateProductCommand` + handler
- [x] `UpdateProductCommand` + handler
- [x] `DeleteProductCommand` + handler
- [x] `GetProductQuery` + handler
- [x] `GetProductsQuery` + handler (paginated + category filter)
- [x] `GetCategoriesQuery` + handler

### Carts
- [x] `CreateCartCommand` + handler
- [x] `UpdateCartCommand` + handler
- [x] `DeleteCartCommand` + handler
- [x] `GetCartQuery` + handler
- [x] `GetCartsQuery` + handler (paginated)

### Users (completing template)
- [x] `UpdateUserCommand` + handler
- [x] `GetUsersQuery` + handler (paginated)

### Common
- [x] `IEventPublisher` in `Common.Events`

---

## Phase 4 — Services Layer `feature/services-layer`

**Progress: 0% (0/8)**

> Rebus for event publishing. Polly for retry. Serilog structured logging.

- [ ] Add Rebus package to Infrastructure project
- [ ] `LogEventPublisher` — implements `IEventPublisher` via Serilog + Polly retry (3x exponential backoff)
- [ ] Register `IEventPublisher` in IoC (`InfrastructureModuleInitializer`)
- [ ] `SaleCreatedEvent` published on sale creation
- [ ] `SaleModifiedEvent` published on sale update
- [ ] `SaleCancelledEvent` published on sale cancellation
- [ ] `ItemCancelledEvent` published on item cancellation
- [ ] Validate Polly retry: log warning on each retry, error on final failure

---

## Phase 5 — Business Rules `feature/business-rules`

**Progress: 100% (10/10)**

> Rich domain: rules live exclusively in entities. FluentResults for domain operations.

- [x] `IDomainEvent` + `IEventPublisher` in `Common.Events`
- [x] `BaseEntity` with domain event dispatch (`AddDomainEvent`, `ClearDomainEvents`)
- [x] `SaleItem` discount rules:
  - [x] Qty < 4 → no discount
  - [x] Qty 4–9 → 10% discount
  - [x] Qty 10–20 → 20% discount
  - [x] Qty > 20 → `Result.Fail`
- [x] `Sale.Cancel()` → raises `SaleCancelledEvent`
- [x] `Sale.CancelItem(productId)` → raises `ItemCancelledEvent`
- [x] `SaleValidator`, `SaleItemValidator`, `ProductValidator`, `CartValidator` (FluentValidation)
- [x] `ISaleRepository`, `IProductRepository`, `ICartRepository` interfaces
- [x] `Rating` value object with invariant validation

---

## Phase 6 — Application Log `feature/app-logging`

**Progress: 0% (0/5)**

> Serilog already configured in template. Add structured log entries at key points.

- [ ] Log `SaleCreatedEvent` with sale number, customer, total amount
- [ ] Log `SaleModifiedEvent` with sale ID and changed fields
- [ ] Log `SaleCancelledEvent` with sale ID and reason
- [ ] Log `ItemCancelledEvent` with sale ID and product ID
- [ ] Log validation errors and domain failures at Warning level

---

## Phase 7 — Tests `feature/tests`

**Progress: 0% (0/8)**

> xUnit + NSubstitute + Bogus (Faker) + FluentAssertions.
> Pattern: Arrange / Act / Assert — one concern per test.
> Coverage target: ≥ 80%.

- [ ] Unit tests — `SaleItem` discount rules (all tiers + boundary + limit)
- [ ] Unit tests — `Sale` aggregate (`AddItem`, `Cancel`, `CancelItem`, `TotalAmount`)
- [ ] Unit tests — `CreateSaleHandler` (success + domain failure)
- [ ] Unit tests — `CancelSaleHandler` (success + not found + already cancelled)
- [ ] Integration tests setup — `WebApplicationFactory` + real PostgreSQL
- [ ] Coverage report configured (`coverlet` + `coverage-report.sh`)
- [ ] Postman collection with all endpoints (saved in `docs/`)
- [ ] Manual CRUD test scenarios documented in `docs/`

---

## Phase 8 — Documentation `feature/documentation`

**Progress: 0% (0/5)**

- [ ] `README.md` complete — setup, prerequisites, how to run, how to test, env vars, endpoints
- [ ] `.env.example` with all required environment variables
- [ ] Docker Compose usage documented (dev + test profiles)
- [ ] API endpoint summary table in README
- [ ] Update `docs/domain.md` with final architecture decisions
