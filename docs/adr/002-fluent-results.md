# ADR 002 — FluentResults over Exceptions for Domain Failures

## Status
Accepted

## Context
Domain operations can fail in expected ways (e.g., cancelling an already cancelled sale, selling more than 20 items). Two common approaches exist: throw exceptions or return discriminated result types.

## Decision
We use **FluentResults** (`Result<T>`) as the return type for all domain operations and application handlers. Exceptions are reserved for truly unexpected failures (infrastructure errors, unhandled cases).

```csharp
// Domain
public Result Cancel()
{
    if (IsCancelled)
        return Result.Fail("Sale is already cancelled.");
    ...
}

// Handler
var result = sale.Cancel();
if (result.IsFailed)
    return Result.Fail<UpdateSaleResult>(result.Errors);
```

## Consequences
**Positive:**
- Expected business failures are explicit in the method signature — callers must handle them
- No stack trace overhead for predictable failures
- Controllers map `Result` to HTTP status codes cleanly via `HandleResult()`
- Errors compose naturally — multiple failures can be collected and returned together

**Negative:**
- Handlers are slightly more verbose than simple throw/catch
- Requires discipline to not mix exceptions and Results inconsistently
