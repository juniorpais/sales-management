[Back to README](../README.md)

## Project Structure

```
root/
├── src/
│   ├── Ambev.DeveloperEvaluation.Domain/
│   │   ├── Common/          # BaseEntity, IDomainEvent
│   │   ├── Entities/        # Sale, SaleItem, Product, Cart, CartItem, User
│   │   ├── Events/          # SaleCreatedEvent, SaleModifiedEvent, SaleCancelledEvent, ItemCancelledEvent
│   │   ├── Repositories/    # ISaleRepository, IProductRepository, ICartRepository, IUserRepository
│   │   ├── Validation/      # FluentValidation validators per entity
│   │   └── ValueObjects/    # Rating
│   │
│   ├── Ambev.DeveloperEvaluation.Application/
│   │   ├── Sales/           # CreateSale, UpdateSale, CancelSale, GetSale, GetSales
│   │   ├── Products/        # CreateProduct, UpdateProduct, DeleteProduct, GetProduct, GetProducts, GetCategories
│   │   ├── Carts/           # CreateCart, UpdateCart, DeleteCart, GetCart, GetCarts
│   │   ├── Users/           # CreateUser, UpdateUser, DeleteUser, GetUser, GetUsers
│   │   └── Auth/            # AuthenticateUser
│   │
│   ├── Ambev.DeveloperEvaluation.ORM/
│   │   ├── Mapping/         # EF Core Fluent API configurations
│   │   ├── Migrations/      # Database migrations
│   │   └── Repositories/    # Repository implementations
│   │
│   ├── Ambev.DeveloperEvaluation.IoC/
│   │   ├── ModuleInitializers/  # DI registration per layer
│   │   └── Services/            # LogEventPublisher (Polly retry)
│   │
│   ├── Ambev.DeveloperEvaluation.Common/
│   │   ├── Events/          # IDomainEvent, IEventPublisher
│   │   ├── Security/        # JWT, password hashing
│   │   ├── Validation/      # ValidationBehavior (MediatR pipeline)
│   │   └── Logging/         # Serilog configuration
│   │
│   └── Ambev.DeveloperEvaluation.WebApi/
│       ├── Features/        # Controllers + Request/Response models per resource
│       ├── Common/          # BaseController, ApiResponse, PaginatedResponse
│       └── Middleware/      # ValidationExceptionMiddleware
│
├── tests/
│   ├── Ambev.DeveloperEvaluation.Unit/
│   │   ├── Domain/          # Entity and business rule tests
│   │   └── Application/     # Handler tests (NSubstitute mocks)
│   │
│   ├── Ambev.DeveloperEvaluation.Integration/
│   │   └── Sales/           # HTTP endpoint tests (WebApplicationFactory)
│   │
│   └── Ambev.DeveloperEvaluation.Functional/
│       └── Sales/           # Complete business flow tests
│
├── docs/
│   ├── domain.md                                # Domain model with Mermaid diagrams
│   ├── general-api.md                           # Pagination, filtering, ordering, error format
│   ├── sales-management.postman_collection.json # Postman collection
│   └── ...
│
├── docker-compose.yml       # PostgreSQL + MongoDB + Redis + WebApi
├── Dockerfile               # Multi-stage build
├── coverage-report.sh       # Coverage report generator
└── .env.example             # Environment variables template
```
