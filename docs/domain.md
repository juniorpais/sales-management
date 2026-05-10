[Back to README](../README.md)

## Domain Documentation

### Aggregates & Entities

```mermaid
classDiagram
    class BaseEntity {
        +Guid Id
        +IReadOnlyCollection~IDomainEvent~ DomainEvents
        +AddDomainEvent(IDomainEvent)
        +ClearDomainEvents()
    }

    class Sale {
        +string SaleNumber
        +DateTime Date
        +Guid CustomerId
        +string CustomerName
        +Guid BranchId
        +string BranchName
        +bool IsCancelled
        +decimal TotalAmount
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +IReadOnlyCollection~SaleItem~ Items
        +Create(...)$ Result~Sale~
        +AddItem(...) Result
        +Update(...) Result
        +Cancel() Result
        +CancelItem(productId) Result
    }

    class SaleItem {
        +Guid SaleId
        +Guid ProductId
        +string ProductName
        +int Quantity
        +decimal UnitPrice
        +decimal Discount
        +decimal TotalAmount
        +bool IsCancelled
        +Create(...)$ Result~SaleItem~
        +UpdateQuantity(qty) Result
        +Cancel()
    }

    class Product {
        +string Title
        +decimal Price
        +string Description
        +string Category
        +string Image
        +Rating Rating
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +Create(...)$ Result~Product~
        +Update(...) Result
    }

    class Cart {
        +Guid UserId
        +string UserName
        +DateTime Date
        +IReadOnlyCollection~CartItem~ Items
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +Create(...)$ Result~Cart~
        +AddItem(...) Result
        +RemoveItem(productId) Result
        +Update(date) Result
    }

    class CartItem {
        +Guid CartId
        +Guid ProductId
        +string ProductName
        +int Quantity
        +Create(...)$ Result~CartItem~
        +UpdateQuantity(qty) Result
    }

    class Rating {
        +decimal Rate
        +decimal Count
    }

    BaseEntity <|-- Sale
    BaseEntity <|-- SaleItem
    BaseEntity <|-- Product
    BaseEntity <|-- Cart
    BaseEntity <|-- CartItem

    Sale "1" *-- "0..*" SaleItem : contains
    Cart "1" *-- "0..*" CartItem : contains
    Product "1" *-- "1" Rating : has
```

---

### External Identities Pattern

> Entities from other domains are referenced by **ID + denormalized name**, never as foreign keys with joined tables. This avoids cross-domain coupling and keeps each bounded context autonomous.

```mermaid
classDiagram
    class Sale {
        +Guid CustomerId
        +string CustomerName
        +Guid BranchId
        +string BranchName
    }

    class SaleItem {
        +Guid ProductId
        +string ProductName
    }

    class Cart {
        +Guid UserId
        +string UserName
    }

    class CartItem {
        +Guid ProductId
        +string ProductName
    }

    class Customer ["Customer (external)"] {
        +Guid Id
        +string Name
    }

    class Branch ["Branch (external)"] {
        +Guid Id
        +string Name
    }

    Sale ..> Customer : references by ID + Name
    Sale ..> Branch : references by ID + Name
    SaleItem ..> Product : references by ID + Name
    Cart ..> User : references by ID + Name
    CartItem ..> Product : references by ID + Name
```

---

### Discount Business Rules

```mermaid
flowchart TD
    A([Item quantity]) --> B{qty > 20?}
    B -- Yes --> C[❌ Result.Fail\nCannot sell more than 20 identical items]
    B -- No --> D{qty >= 10?}
    D -- Yes --> E[✅ 20% discount]
    D -- No --> F{qty >= 4?}
    F -- Yes --> G[✅ 10% discount]
    F -- No --> H[✅ No discount]
```

---

### Domain Events

```mermaid
sequenceDiagram
    participant Client
    participant Handler
    participant Sale
    participant IEventPublisher

    Client->>Handler: CreateSaleCommand
    Handler->>Sale: Sale.Create(...)
    Sale-->>Sale: AddDomainEvent(SaleCreatedEvent)
    Handler->>Repository: SaveAsync(sale)
    Handler->>IEventPublisher: PublishAsync(SaleCreatedEvent)
    IEventPublisher-->>Handler: logged ✓

    Client->>Handler: CancelSaleCommand
    Handler->>Sale: sale.Cancel()
    Sale-->>Sale: AddDomainEvent(SaleCancelledEvent)
    Handler->>Repository: UpdateAsync(sale)
    Handler->>IEventPublisher: PublishAsync(SaleCancelledEvent)
    IEventPublisher-->>Handler: logged ✓
```

---

### Architecture Layers

```mermaid
graph TD
    subgraph WebApi
        WA[Controllers\nRequests / Responses\nAutoMapper Profiles]
    end

    subgraph Application
        AP[Commands / Queries\nHandlers - MediatR\nFluentResults\nIEventPublisher]
    end

    subgraph Domain
        DO[Entities\nValue Objects\nDomain Events\nRepository Interfaces\nFluentValidation]
    end

    subgraph Infrastructure
        IN[EF Core\nRepositories\nLogEventPublisher\nMigrations]
    end

    WebApi --> Application
    Application --> Domain
    Infrastructure --> Domain

    WebApi -.->|DI only| Infrastructure
```

---

### Key Design Decisions

| Decision | Choice | Reason |
|---|---|---|
| Return type for domain ops | `Result<T>` (FluentResults) | Explicit failure without exceptions for expected business errors |
| Validation | FluentValidation on entities | Declarative, testable, reusable rules |
| Domain events dispatch | Collected in entity, published after persist | Ensures no events fire if save fails |
| Cross-domain references | External Identity (ID + Name) | Avoids coupling between bounded contexts |
| ORM access | Private setters + private constructor | EF Core reflects; domain controls mutations |
