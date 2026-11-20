using MolTol.Models;
namespace MolTol.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(
        int? categoryId = null,
        string? searchTerm = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<int> AddAsync(
        Product product,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Product product,
        CancellationToken cancellationToken = default);

    Task<bool> SetActiveAsync(
        int productId,
        bool isActive,
        CancellationToken cancellationToken = default);
}
