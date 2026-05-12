using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DefaultContext _context;

    public ProductRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(int page, int size, string? order = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();
        var totalCount = await query.CountAsync(cancellationToken);
        query = query.ApplyOrdering(order);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetByCategoryAsync(string category, int page, int size, string? order = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Products.Where(p => p.Category == category);
        var totalCount = await query.CountAsync(cancellationToken);
        query = query.ApplyOrdering(order);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.Select(p => p.Category).Distinct().ToListAsync(cancellationToken);
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(id, cancellationToken);
        if (product is null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
