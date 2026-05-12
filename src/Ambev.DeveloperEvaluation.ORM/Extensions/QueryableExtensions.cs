using System.Linq.Expressions;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.ORM.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Applies dynamic ordering to an IQueryable based on a comma-separated order string.
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable to order</param>
    /// <param name="order">Order string in format: "fieldName asc" or "fieldName desc", multiple fields separated by commas.
    /// Example: "date desc, saleNumber asc". Field names should be in camelCase and will be mapped to PascalCase entity properties.</param>
    /// <returns>The ordered queryable</returns>
    public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
        {
            // Fall back to ordering by Id ascending
            return ApplyOrderByProperty(query, "Id", true, true);
        }

        // Split by comma to handle multiple order clauses
        var orderClauses = order.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        bool isFirst = true;
        foreach (var clause in orderClauses)
        {
            var parts = clause.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;

            var fieldName = parts[0];
            var isAscending = parts.Length == 1 || !parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            // Convert camelCase to PascalCase
            var propertyName = ToPascalCase(fieldName);

            // Try to apply ordering for this property
            var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            
            if (property != null)
            {
                query = ApplyOrderByProperty(query, property.Name, isAscending, isFirst);
                isFirst = false;
            }
            // If property doesn't exist, silently ignore it
        }

        // If no valid ordering was applied, fall back to Id
        if (isFirst)
        {
            query = ApplyOrderByProperty(query, "Id", true, true);
        }

        return query;
    }

    private static IQueryable<T> ApplyOrderByProperty<T>(IQueryable<T> query, string propertyName, bool isAscending, bool isFirst)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var lambda = Expression.Lambda(property, parameter);

        string methodName;
        if (isFirst)
        {
            methodName = isAscending ? "OrderBy" : "OrderByDescending";
        }
        else
        {
            methodName = isAscending ? "ThenBy" : "ThenByDescending";
        }

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(T), property.Type },
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(resultExpression);
    }

    private static string ToPascalCase(string camelCase)
    {
        if (string.IsNullOrEmpty(camelCase))
            return camelCase;

        return char.ToUpperInvariant(camelCase[0]) + camelCase.Substring(1);
    }
}
