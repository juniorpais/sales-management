using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentResults;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Product : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string Image { get; private set; } = string.Empty;
    public Rating Rating { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Product() { }

    public static Result<Product> Create(string title, decimal price, string description, string category, string image, Rating rating)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(title)) errors.Add("Title is required.");
        if (price <= 0) errors.Add("Price must be greater than zero.");
        if (string.IsNullOrWhiteSpace(category)) errors.Add("Category is required.");

        if (errors.Count > 0)
            return Result.Fail<Product>(errors);

        return Result.Ok(new Product
        {
            Id = Guid.NewGuid(),
            Title = title,
            Price = price,
            Description = description,
            Category = category,
            Image = image,
            Rating = rating,
            CreatedAt = DateTime.UtcNow
        });
    }

    public Result Update(string title, decimal price, string description, string category, string image, Rating rating)
    {
        if (string.IsNullOrWhiteSpace(title)) return Result.Fail("Title is required.");
        if (price <= 0) return Result.Fail("Price must be greater than zero.");
        if (string.IsNullOrWhiteSpace(category)) return Result.Fail("Category is required.");

        Title = title;
        Price = price;
        Description = description;
        Category = category;
        Image = image;
        Rating = rating;
        UpdatedAt = DateTime.UtcNow;

        return Result.Ok();
    }
}
