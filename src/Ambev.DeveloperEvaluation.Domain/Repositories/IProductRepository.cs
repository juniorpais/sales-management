using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(int page, int size, string? order = null, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> Items, int TotalCount)> GetByCategoryAsync(string category, int page, int size, string? order = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
