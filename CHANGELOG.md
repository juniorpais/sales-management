# Changelog

All notable changes to this project are documented here.

---

## [1.0.0] — 2026-05-11

### Phase 9 — Documentation
- Complete README with setup, endpoints, business rules and project structure
- Architecture Decision Records (ADR 001–005)
- API error reference guide
- `.env.example` for environment configuration
- Updated `docs/project-structure.md`
- macOS coverage report script (`coverage-report-macos.sh`)

### Phase 8 — Tests
- **Unit**: 77 tests covering `SaleItem` discount rules, `Sale` aggregate behavior, `CreateSaleHandler` and `CancelSaleHandler`
- **Integration**: 9 tests covering all Sales API endpoints via `WebApplicationFactory`
- **Functional**: 5 tests covering complete sale lifecycle and discount tier flows
- Removed unused files (duplicate Dockerfile, empty docker-compose.override, stale .http file)

### Phase 7 — Application Log
- Enriched `LogEventPublisher` with structured Serilog logs per domain event type
- Added structured logging to `ValidationExceptionMiddleware` (warnings on validation failures)

### Phase 6 — API Layer
- `SalesController` — full CRUD (POST, GET, GET list, PUT, DELETE/cancel)
- `ProductsController` — CRUD + `/categories` + `/category/{category}`
- `CartsController` — full CRUD
- `UsersController` — completed with `GET` (paginated) and `PUT`
- `BaseController.HandleResult<T>()` — maps `FluentResults` to HTTP status codes
- `JsonStringEnumConverter` — enums accepted as strings in JSON requests
- Postman collection with all endpoints and discount business rule scenarios

### Phase 5 — Business Rules
- Rich domain: `SaleItem` discount tiers (0%, 10%, 20%) enforced in entity
- `Sale.Cancel()` and `Sale.CancelItem()` with domain event dispatch
- `FluentValidation` validators for `Sale`, `SaleItem`, `Product`, `Cart`
- `FluentResults` (`Result<T>`) for all domain operations

### Phase 4 — Services Layer
- `LogEventPublisher` implementing `IEventPublisher` with Polly retry (3x exponential backoff)
- Registered `IEventPublisher` in IoC

### Phase 3 — Data Manipulation Layer
- MediatR CQRS handlers for Sales, Products, Carts and Users
- `IEventPublisher` abstraction in `Common.Events`
- `IDomainEvent` moved to `Common.Events`
- `UpdateAsync` and `GetAllAsync` added to `IUserRepository`

### Phase 2 — Database + ORM Mapping
- EF Core Fluent API configurations for Sale, SaleItem, Product, Cart, CartItem
- `SaleRepository`, `ProductRepository`, `CartRepository` with pagination
- Migration `AddSaleProductCart`
- PostgreSQL connection string aligned with Docker Compose

### Phase 1 — Project Setup
- Moved template structure from `template/backend/` to repository root
- `.gitignore`, `.editorconfig`, `.dockerignore` configured
- `DEVELOPMENT.md` with phases, Git Flow and semantic commit convention
- Renamed `docs/` folder (was `.doc/`) and fixed all internal links
