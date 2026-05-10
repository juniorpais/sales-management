# Development Plan — Sales Management API

## Progress Overview

| Phase | Description | Branch | Status | Progress |
|-------|-------------|--------|--------|----------|
| 1 | Project Setup | `main` | ✅ Done | 100% |
| 2 | Domain Layer | `feature/domain-layer` | ✅ Done | 100% |
| 3 | Application Layer | `feature/application-layer` | ⬜ Pending | 0% |
| 4 | Infrastructure Layer | `feature/infrastructure-layer` | ⬜ Pending | 0% |
| 5 | API Layer | `feature/api-layer` | ⬜ Pending | 0% |
| 6 | Events | `feature/domain-events` | ⬜ Pending | 0% |
| 7 | Tests | `feature/tests` | ⬜ Pending | 0% |
| 8 | Documentation | `feature/documentation` | ⬜ Pending | 0% |

**Overall Progress: 29% (22 / 75 tasks completed)**

---

## Git Flow

```
main
 └── develop
      ├── feature/domain-layer
      ├── feature/application-layer
      ├── feature/infrastructure-layer
      ├── feature/api-layer
      ├── feature/domain-events
      ├── feature/tests
      └── feature/documentation
```

> Each `feature/*` branch is created from `develop` and merged back via PR.  
> Releases are cut from `develop` → `release/x.y.z` → merged into `main` + `develop`.  
> Hotfixes branch from `main` → `hotfix/description` → merged into `main` + `develop`.

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

- [x] Move template structure to repository root (`src/`, `tests/`)
- [x] Validate solution file references
- [x] Docker Compose baseline (PostgreSQL + app)
- [x] `.editorconfig` and `.dockerignore` in place

---

## Phase 2 — Domain Layer `feature/domain-layer`

**Progress: 0% (0/18)**

> Rich domain: business rules, invariants, and validation live exclusively in the domain entities — never in handlers or controllers.  
> Uses **FluentValidation** for entity validators.  
> Uses **FluentResults** (`Result<T>`) as return type for all domain operations.

### Foundation

- [x] Enhance `BaseEntity` with domain event dispatch (`AddDomainEvent`)
- [x] Create `IDomainEvent` marker interface
- [x] Create `DomainException` extensions for business rule violations

### Sale Aggregate

- [x] `Sale` entity (SaleNumber, Date, CustomerId, CustomerName, BranchId, BranchName, IsCancelled, Items, TotalAmount)
- [x] `SaleItem` entity (ProductId, ProductName, Quantity, UnitPrice, Discount, TotalAmount)
- [x] Discount business rules inside `SaleItem`:
  - [x] Qty < 4 → no discount
  - [x] Qty 4–9 → 10% discount
  - [x] Qty 10–20 → 20% discount
  - [x] Qty > 20 → `Result.Fail("Cannot sell more than 20 identical items")`
- [x] `Sale.Cancel()` method → sets `IsCancelled = true`, raises `SaleCancelledEvent`
- [x] `Sale.CancelItem(productId)` → raises `ItemCancelledEvent`
- [x] `SaleValidator` (FluentValidation)
- [x] `SaleItemValidator` (FluentValidation)

### Supporting Entities

- [x] `Product` entity (Title, Price, Description, Category, Image, Rating)
- [x] `ProductValidator` (FluentValidation)
- [x] `Cart` entity (UserId, Date, Items)
- [x] `CartItem` value object (ProductId, Quantity)
- [x] `CartValidator` (FluentValidation)

### Repository Interfaces

- [x] `ISaleRepository` (GetById, GetAll, Create, Update, Delete)
- [x] `IProductRepository`
- [x] `ICartRepository`

---

## Phase 3 — Application Layer `feature/application-layer`

**Progress: 0% (0/19)**

> Uses **MediatR** (CQRS pattern).  
> All handlers return `Result<T>` from **FluentResults**.  
> **AutoMapper** profiles for Command → Entity and Entity → Result.

### Infrastructure Abstractions

- [ ] `IEventPublisher` interface (`PublishAsync<T>(T event)`)

### Sales

- [ ] `CreateSaleCommand` + `CreateSaleHandler` → returns `Result<CreateSaleResult>`
- [ ] `UpdateSaleCommand` + `UpdateSaleHandler` → returns `Result<UpdateSaleResult>`
- [ ] `CancelSaleCommand` + `CancelSaleHandler` → returns `Result`
- [ ] `GetSaleQuery` + `GetSaleHandler` → returns `Result<GetSaleResult>`
- [ ] `GetSalesQuery` + `GetSalesHandler` (paginated) → returns `Result<PaginatedList<GetSaleResult>>`
- [ ] AutoMapper profile: `SaleMappingProfile`

### Products

- [ ] `CreateProductCommand` + handler
- [ ] `UpdateProductCommand` + handler
- [ ] `DeleteProductCommand` + handler
- [ ] `GetProductQuery` + handler
- [ ] `GetProductsQuery` + handler (paginated + filter by category)
- [ ] AutoMapper profile: `ProductMappingProfile`

### Carts

- [ ] `CreateCartCommand` + handler
- [ ] `UpdateCartCommand` + handler
- [ ] `DeleteCartCommand` + handler
- [ ] `GetCartQuery` + handler
- [ ] `GetCartsQuery` + handler (paginated)
- [ ] AutoMapper profile: `CartMappingProfile`

### Users (complete missing operations)

- [ ] `UpdateUserCommand` + handler
- [ ] `GetUsersQuery` + handler (paginated)

---

## Phase 4 — Infrastructure Layer `feature/infrastructure-layer`

**Progress: 0% (0/13)**

> EF Core + PostgreSQL.  
> Repositories implement interfaces from Domain.  
> Event publisher logs via `ILogger` with Polly retry policy.

### EF Core Mappings

- [ ] `SaleConfiguration` (Fluent API — table, columns, owned collections)
- [ ] `SaleItemConfiguration`
- [ ] `ProductConfiguration`
- [ ] `CartConfiguration`
- [ ] `CartItemConfiguration`
- [ ] Register all configurations in `DefaultContext`

### Repositories

- [ ] `SaleRepository` (implements `ISaleRepository`) — with pagination + filtering + ordering
- [ ] `ProductRepository` (implements `IProductRepository`)
- [ ] `CartRepository` (implements `ICartRepository`)

### Event Publisher

- [ ] `LogEventPublisher` — implements `IEventPublisher`, logs via Serilog, Polly retry (3x exponential backoff)

### Migrations

- [ ] EF migration for Sales, SaleItems, Products, Carts, CartItems tables
- [ ] Validate `docker-compose up` runs migrations on startup

### IoC

- [ ] Register new repositories and `IEventPublisher` in `InfrastructureModuleInitializer`

---

## Phase 5 — API Layer `feature/api-layer`

**Progress: 0% (0/10)**

> All responses use the existing `ApiResponseWithData<T>` and `PaginatedResponse<T>` wrappers.  
> Errors follow the `{ type, error, detail }` format from `general-api.md`.  
> Supports `_page`, `_size`, `_order`, field filters, `_min*`, `_max*` query params.

### Controllers

- [ ] `SalesController` — POST, GET/{id}, GET (paginated), PUT/{id}, DELETE/{id}
- [ ] `ProductsController` — POST, GET/{id}, GET (paginated), PUT/{id}, DELETE/{id}, GET/categories, GET/category/{category}
- [ ] `CartsController` — POST, GET/{id}, GET (paginated), PUT/{id}, DELETE/{id}
- [ ] `UsersController` — complete with GET (paginated) and PUT/{id}

### Request / Response Models

- [ ] Sales: `CreateSaleRequest`, `UpdateSaleRequest`, `SaleResponse`, `SaleItemResponse`
- [ ] Products: `CreateProductRequest`, `UpdateProductRequest`, `ProductResponse`
- [ ] Carts: `CreateCartRequest`, `UpdateCartRequest`, `CartResponse`
- [ ] Users: `UpdateUserRequest`
- [ ] AutoMapper profiles for all Request → Command and Result → Response mappings

### Middleware

- [ ] Map `Result.Fail` reasons to appropriate HTTP status codes (400, 404, 422)

---

## Phase 6 — Domain Events `feature/domain-events`

**Progress: 0% (0/6)**

- [ ] `SaleCreatedEvent` (raised on `Sale` creation)
- [ ] `SaleModifiedEvent` (raised on `Sale` update)
- [ ] `SaleCancelledEvent` (raised on `Sale.Cancel()`)
- [ ] `ItemCancelledEvent` (raised on `Sale.CancelItem()`)
- [ ] Dispatch events after handler persists aggregate (`PublishAsync` in handlers)
- [ ] Polly retry validated: logs warning on each retry, error on final failure

---

## Phase 7 — Tests `feature/tests`

**Progress: 0% (0/5)**

> xUnit + NSubstitute + Bogus + **FluentAssertions**.  
> Pattern: Arrange / Act / Assert, one concern per test.  
> Coverage target: ≥ 80%.

- [ ] Unit tests — `SaleItem` discount rules (all tiers + boundary + limit violation)
- [ ] Unit tests — `Sale` aggregate (`Cancel`, `CancelItem`, total calculation)
- [ ] Unit tests — handlers (CreateSale, UpdateSale, CancelSale, GetSale)
- [ ] Integration tests setup — `WebApplicationFactory` + real PostgreSQL (Docker Compose profile)
- [ ] Coverage report configured (`coverlet` + `coverage-report.sh`)

---

## Phase 8 — Documentation `feature/documentation`

**Progress: 0% (0/5)**

- [ ] `README.md` — complete setup, prerequisites, how to run, how to test
- [ ] Environment variables documented (`.env.example`)
- [ ] Docker Compose usage documented (dev + test profiles)
- [ ] API endpoint summary table in README
- [ ] Postman collection or `.http` file with example requests for all endpoints
