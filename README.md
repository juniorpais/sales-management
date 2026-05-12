# Sales Management API

A RESTful API for managing sales records, built with .NET 8, DDD, CQRS, and Clean Architecture.

## Table of Contents

- [Use Case](#use-case)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Running the API](#running-the-api)
- [Running Tests](#running-tests)
- [API Endpoints](#api-endpoints)
- [Business Rules](#business-rules)
- [Project Structure](#project-structure)
- [Documentation](#documentation)

---

## Use Case

Sales Management API for the DeveloperStore team. Handles complete sales records including products, quantities, unit prices, discounts, and cancellations. Implements the **External Identities** pattern for cross-domain references (Customer, Branch, Product).

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# / .NET 8 |
| Architecture | DDD + CQRS + Clean Architecture |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL 13 |
| Messaging | MediatR 12 |
| Mapping | AutoMapper 13 |
| Validation | FluentValidation 11 |
| Results | FluentResults 3 |
| Events | Rebus + RabbitMQ + Polly (retry) |
| Logging | Serilog |
| Tests | xUnit + NSubstitute + Bogus + FluentAssertions |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker](https://www.docker.com/get-started) + Docker Compose
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet): `dotnet tool install --global dotnet-ef`
- [Postman](https://www.postman.com/) (optional — for manual testing)

---

## Getting Started

**1. Clone the repository**

```bash
git clone https://github.com/juniorpais/sales-management.git
cd sales-management
```

**2. Start the infrastructure**

```bash
docker-compose up -d ambev.developerevaluation.database ambev.developerevaluation.rabbitmq
```

> RabbitMQ management UI: `http://localhost:15672` (user: `developer` / pass: `ev@luAt10n`)

**3. Apply migrations**

```bash
dotnet ef database update \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi \
  --context DefaultContext
```

> **Note:** If you have .NET 9+ installed alongside .NET 8, prefix the command with `DOTNET_ROLL_FORWARD=Major`.

**4. Run the API**

```bash
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

The API will be available at:
- HTTP: `http://localhost:5119`
- Swagger UI: `http://localhost:5119/swagger`

---

### Alternative — Run everything with Docker Compose

If you prefer not to install .NET SDK locally, you can run the full stack with Docker:

**1. Build and start all services**

```bash
docker-compose up --build
```

This starts PostgreSQL, RabbitMQ, and the WebApi container.

**2. Apply migrations** *(first run only)*

```bash
docker exec -it ambev_developer_evaluation_webapi \
  dotnet ef database update \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi \
  --context DefaultContext
```

> Alternatively, the API applies pending migrations automatically on startup if `EnsureCreated` is configured.

**3. Access the API**

- Swagger UI: `http://localhost:8080/swagger`
- RabbitMQ Management: `http://localhost:15672` (user: `developer` / pass: `ev@luAt10n`)

---

## Running Tests

**Unit tests**

```bash
DOTNET_ROLL_FORWARD=Major dotnet test tests/Ambev.DeveloperEvaluation.Unit
```

**Integration tests** *(requires PostgreSQL running)*

```bash
DOTNET_ROLL_FORWARD=Major dotnet test tests/Ambev.DeveloperEvaluation.Integration
```

**Functional tests** *(requires PostgreSQL running)*

```bash
DOTNET_ROLL_FORWARD=Major dotnet test tests/Ambev.DeveloperEvaluation.Functional
```

**All tests**

```bash
DOTNET_ROLL_FORWARD=Major dotnet test Ambev.DeveloperEvaluation.sln
```

**Coverage report**

```bash
# Linux/Windows
bash coverage-report.sh

# macOS
bash coverage-report-macos.sh
```

---

## API Endpoints

### Authentication

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/login` | Authenticate and receive JWT token |

### Users

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/users` | Create user |
| GET | `/api/users` | List users (paginated) |
| GET | `/api/users/{id}` | Get user by ID |
| PUT | `/api/users/{id}` | Update user |
| DELETE | `/api/users/{id}` | Delete user |

### Products

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/products` | Create product |
| GET | `/api/products` | List products (paginated) |
| GET | `/api/products/{id}` | Get product by ID |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |
| GET | `/api/products/categories` | List all categories |
| GET | `/api/products/category/{category}` | Products by category |

### Carts

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/carts` | Create cart |
| GET | `/api/carts` | List carts (paginated) |
| GET | `/api/carts/{id}` | Get cart by ID |
| PUT | `/api/carts/{id}` | Update cart |
| DELETE | `/api/carts/{id}` | Delete cart |

### Sales

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/sales` | Create sale |
| GET | `/api/sales` | List sales (paginated) |
| GET | `/api/sales/{id}` | Get sale by ID |
| PUT | `/api/sales/{id}` | Update sale |
| DELETE | `/api/sales/{id}` | Cancel sale |

### Pagination, Filtering & Ordering

All list endpoints support:

```
GET /api/sales?_page=1&_size=10&_order=date desc
GET /api/products?_page=2&_size=20&_order=price asc
```

---

## Business Rules

Quantity-based discount tiers applied automatically per item:

| Quantity | Discount |
|---|---|
| 1 – 3 | No discount |
| 4 – 9 | 10% |
| 10 – 20 | 20% |
| > 20 | ❌ Not allowed |

---

## Project Structure

```
├── src/
│   ├── Ambev.DeveloperEvaluation.Domain/        # Entities, business rules, events, interfaces
│   ├── Ambev.DeveloperEvaluation.Application/   # CQRS handlers, validators, AutoMapper profiles
│   ├── Ambev.DeveloperEvaluation.ORM/           # EF Core mappings, repositories, migrations
│   ├── Ambev.DeveloperEvaluation.IoC/           # Dependency injection, RebusEventPublisher
│   ├── Ambev.DeveloperEvaluation.Common/        # Shared abstractions (IEventPublisher, IDomainEvent)
│   └── Ambev.DeveloperEvaluation.WebApi/        # Controllers, middleware, request/response models
├── tests/
│   ├── Ambev.DeveloperEvaluation.Unit/          # Unit tests (domain + handlers)
│   ├── Ambev.DeveloperEvaluation.Integration/   # Integration tests (API endpoints)
│   └── Ambev.DeveloperEvaluation.Functional/    # Functional tests (complete business flows)
├── docs/
│   ├── domain.md                                # Domain model with Mermaid diagrams
│   ├── sales-management.postman_collection.json # Postman collection
│   └── ...                                      # API specs and architecture docs
├── docker-compose.yml
├── Dockerfile
└── coverage-report.sh
```

---

## Documentation

- [Domain Model & Architecture](docs/domain.md)
- [API General Definitions](docs/general-api.md)
- [Sales API](docs/sales-api.md)
- [Products API](docs/products-api.md)
- [Carts API](docs/carts-api.md)
- [Users API](docs/users-api.md)
- [Auth API](docs/auth-api.md)
- [Architecture Decision Records](docs/adr/) — includes deployment strategy (Docker Compose → Kubernetes)
- [API Error Reference](docs/error-reference.md)
- [Postman Collection](docs/sales-management.postman_collection.json)
- [Changelog](CHANGELOG.md)
