[Back to README](../README.md)

## API Error Reference

All errors follow the standard response format:

```json
{
  "success": false,
  "message": "string",
  "errors": []
}
```

---

### HTTP Status Codes

| Status | Meaning | When it occurs |
|---|---|---|
| `400 Bad Request` | Validation or business rule failure | Invalid input, duplicate sale number, quantity > 20, cancelling already cancelled sale |
| `404 Not Found` | Resource not found | Sale/Product/Cart/User ID does not exist |
| `500 Internal Server Error` | Unexpected server error | Unhandled exception |

---

### Validation Errors (400)

Triggered by FluentValidation before the handler executes.

```json
{
  "success": false,
  "message": "Validation Failed",
  "errors": [
    {
      "error": "SaleNumber",
      "detail": "'Sale Number' must not be empty."
    }
  ]
}
```

**Common validation errors:**

| Field | Rule | Message |
|---|---|---|
| `saleNumber` | Required, max 50 chars | `'Sale Number' must not be empty.` |
| `date` | Required, not in future | `'Date' must be less than or equal to current date.` |
| `customerId` | Required UUID | `'Customer Id' must not be empty.` |
| `items` | At least one item | `Sale must have at least one item.` |
| `items[].quantity` | 1–20 | `'Quantity' must be between 1 and 20.` |
| `items[].unitPrice` | Greater than 0 | `'Unit Price' must be greater than '0'.` |

---

### Business Rule Errors (400)

Triggered by domain logic after validation passes.

| Scenario | Message |
|---|---|
| Duplicate sale number | `Sale with number 'SALE-001' already exists.` |
| Quantity above 20 | `Cannot sell more than 20 identical items.` |
| Cancelling already cancelled sale | `Sale is already cancelled.` |
| Updating a cancelled sale | `Cannot update a cancelled sale.` |
| Cancelling item not in sale | `Item not found or already cancelled.` |

---

### Not Found Errors (404)

```json
{
  "success": false,
  "message": "Sale with ID '00000000-0000-0000-0000-000000000000' not found.",
  "errors": []
}
```

| Resource | Message pattern |
|---|---|
| Sale | `Sale with ID '{id}' not found.` |
| Product | `Product with ID '{id}' not found.` |
| Cart | `Cart with ID '{id}' not found.` |
| User | `User with ID '{id}' not found.` |
