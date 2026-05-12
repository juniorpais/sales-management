# Development Plan — Sales Management API

## Progress Overview

| Phase | Description | Branch | Status | Progress |
|-------|-------------|--------|--------|----------|
| 1 | Project Setup | `main` | ✅ Done | 100% |
| 2 | Database + ORM Mapping | `feature/database-orm` | ✅ Done | 100% |
| 3 | Data Manipulation Layer | `feature/data-manipulation` | ✅ Done | 100% |
| 4 | Services Layer | `feature/database-orm` | ✅ Done | 100% |
| 5 | Business Rules | `feature/business-rules` | ✅ Done | 100% |
| 6 | API Layer | `feature/api-layer` | ✅ Done | 100% |
| 7 | Application Log | `feature/app-logging` | ✅ Done | 100% |
| 8 | Tests | `feature/tests` | ✅ Done | 100% |
| 9 | Documentation | `feature/documentation` | ✅ Done | 100% |

**Overall Progress: 100% (80 / 80 tasks completed)**

---

## Git Flow

```
main
 ├── feature/business-rules      (domain entities, events, validators)
 ├── feature/data-manipulation   (CQRS handlers, MediatR, AutoMapper)
 ├── feature/database-orm        (EF Core mappings, repositories, migrations, LogEventPublisher)
 ├── feature/api-layer           (controllers, request/response, middleware)
 ├── feature/app-logging         (structured Serilog logs per event)
 ├── feature/tests               (unit, integration and functional tests)
 └── feature/documentation       (README, ADRs, CHANGELOG, error reference)
```

> Each `feature/*` branch was merged into `main` via `--no-ff` merge.

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

## Phase 4 — Services Layer `feature/database-orm`

**Progress: 100% (8/8)**

> Polly for retry. Serilog structured logging. IEventPublisher abstraction.

- [x] Add Polly package to IoC project
- [x] `LogEventPublisher` — implements `IEventPublisher` via Serilog + Polly retry (3x exponential backoff)
- [x] Register `IEventPublisher` in IoC (`InfrastructureModuleInitializer`)
- [x] `SaleCreatedEvent` published on sale creation
- [x] `SaleModifiedEvent` published on sale update
- [x] `SaleCancelledEvent` published on sale cancellation
- [x] `ItemCancelledEvent` published on item cancellation
- [x] Polly retry: log warning on each retry, error on final failure

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

## Phase 6 — API Layer `feature/api-layer`

**Progress: 100% (10/10)**

> RESTful controllers. AutoMapper request/response profiles. FluentResults mapped to HTTP status codes.

- [x] `BaseController` with `HandleResult<T>` helper for FluentResults
- [x] `SalesController` — POST, GET/{id}, GET (paginated), PUT/{id}, DELETE/{id}
- [x] `ProductsController` — POST, GET/{id}, GET (paginated), GET/categories, GET/category/{category}, PUT/{id}, DELETE/{id}
- [x] `CartsController` — POST, GET/{id}, GET (paginated), PUT/{id}, DELETE/{id}
- [x] `UsersController` — complete with GET (paginated) and PUT/{id}
- [x] Request/Response models for all resources
- [x] AutoMapper profiles for all Request → Command mappings
- [x] `JsonStringEnumConverter` — enums accepted as strings in JSON
- [x] PostgreSQL connection string aligned with Docker Compose
- [x] Postman collection in `docs/`

---

## Phase 7 — Application Log `feature/app-logging`

**Progress: 100% (5/5)**

> Serilog already configured in template. Structured log entries at key points.

- [x] Log `SaleCreatedEvent` with sale number and occurred at
- [x] Log `SaleModifiedEvent` with sale ID
- [x] Log `SaleCancelledEvent` with sale ID
- [x] Log `ItemCancelledEvent` with sale ID and product ID
- [x] Log validation errors at Warning level in `ValidationExceptionMiddleware`

---

## Phase 8 — Tests `feature/tests`

**Progress: 100% (8/8)**

> xUnit + NSubstitute + Bogus (Faker) + FluentAssertions.
> Pattern: Arrange / Act / Assert — one concern per test.
> 91 tests total: 77 unit + 9 integration + 5 functional.

- [x] Unit tests — `SaleItem` discount rules (all tiers + boundary + limit)
- [x] Unit tests — `Sale` aggregate (`AddItem`, `Cancel`, `CancelItem`, `TotalAmount`)
- [x] Unit tests — `CreateSaleHandler` (success + duplicate + event published)
- [x] Unit tests — `CancelSaleHandler` (success + not found + already cancelled)
- [x] Integration tests — Sales API endpoints via `WebApplicationFactory`
- [x] Functional tests — complete sale lifecycle and discount tier flows
- [x] Coverage report configured (`coverlet` + `coverage-report.sh` / `coverage-report-macos.sh`)
- [x] Postman collection with all endpoints (saved in `docs/`)

---

## Phase 9 — Documentation `feature/documentation`

**Progress: 100% (8/8)**

- [x] `README.md` — setup, prerequisites, run instructions, endpoints table, business rules
- [x] `.env.example` with all required environment variables
- [x] Docker Compose simplified to WebApi + PostgreSQL only
- [x] API endpoint summary table in README
- [x] `docs/domain.md` — domain model with Mermaid diagrams
- [x] Architecture Decision Records (ADR 001–005)
- [x] API error reference guide (`docs/error-reference.md`)
- [x] `CHANGELOG.md` with all development phases
