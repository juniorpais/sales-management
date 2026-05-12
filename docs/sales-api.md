[Back to README](../README.md)

### Sales

#### GET /sales
- Description: Retrieve a paginated list of sales
- Query Parameters:
  - `_page` (optional): Page number for pagination (default: 1)
  - `_size` (optional): Number of items per page (default: 10)
  - `_order` (optional): Ordering of results (e.g., "date desc, totalAmount asc")
- Response:
  ```json
  {
    "data": [
      {
        "id": "uuid",
        "saleNumber": "string",
        "date": "datetime",
        "customerId": "uuid",
        "customerName": "string",
        "branchId": "uuid",
        "branchName": "string",
        "totalAmount": "number",
        "isCancelled": "boolean",
        "items": [
          {
            "id": "uuid",
            "productId": "uuid",
            "productName": "string",
            "quantity": "integer",
            "unitPrice": "number",
            "discount": "number",
            "totalAmount": "number",
            "isCancelled": "boolean"
          }
        ]
      }
    ],
    "totalItems": "integer",
    "currentPage": "integer",
    "totalPages": "integer"
  }
  ```

#### POST /sales
- Description: Create a new sale. Discount tiers are applied automatically per item based on quantity.
- Business Rules:
  - Quantity 1–3: no discount
  - Quantity 4–9: 10% discount
  - Quantity 10–20: 20% discount
  - Quantity > 20: rejected with 400 Bad Request
- Request Body:
  ```json
  {
    "saleNumber": "string",
    "date": "datetime",
    "customerId": "uuid",
    "customerName": "string",
    "branchId": "uuid",
    "branchName": "string",
    "items": [
      {
        "productId": "uuid",
        "productName": "string",
        "quantity": "integer",
        "unitPrice": "number"
      }
    ]
  }
  ```
- Response:
  ```json
  {
    "id": "uuid",
    "saleNumber": "string",
    "date": "datetime",
    "customerId": "uuid",
    "customerName": "string",
    "branchId": "uuid",
    "branchName": "string",
    "totalAmount": "number",
    "isCancelled": "boolean",
    "items": [
      {
        "id": "uuid",
        "productId": "uuid",
        "productName": "string",
        "quantity": "integer",
        "unitPrice": "number",
        "discount": "number",
        "totalAmount": "number",
        "isCancelled": "boolean"
      }
    ]
  }
  ```

#### GET /sales/{id}
- Description: Retrieve a specific sale by ID
- Path Parameters:
  - `id`: Sale UUID
- Response: Same as single item in GET /sales

#### PUT /sales/{id}
- Description: Update a sale's header and items. Cannot update a cancelled sale.
- Path Parameters:
  - `id`: Sale UUID
- Request Body:
  ```json
  {
    "date": "datetime",
    "customerId": "uuid",
    "customerName": "string",
    "branchId": "uuid",
    "branchName": "string",
    "items": [
      {
        "productId": "uuid",
        "productName": "string",
        "quantity": "integer",
        "unitPrice": "number"
      }
    ]
  }
  ```
- Response: Updated sale object (same as POST response)

#### DELETE /sales/{id}
- Description: Cancel a sale (soft delete — sets `isCancelled = true`). Publishes `SaleCancelledEvent` to RabbitMQ.
- Path Parameters:
  - `id`: Sale UUID
- Response:
  ```json
  {
    "success": "boolean",
    "message": "string"
  }
  ```

#### Domain Events
Each write operation publishes a domain event to RabbitMQ (queue: `sales-management`):

| Operation | Event |
|---|---|
| POST /sales | `SaleCreatedEvent` |
| PUT /sales/{id} | `SaleModifiedEvent` |
| DELETE /sales/{id} | `SaleCancelledEvent` |
| Item cancelled | `ItemCancelledEvent` |

<br>
<div style="display: flex; justify-content: space-between;">
  <a href="./general-api.md">Previous: General API</a>
  <a href="./products-api.md">Next: Products API</a>
</div>
