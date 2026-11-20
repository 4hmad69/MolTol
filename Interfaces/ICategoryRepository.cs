using MolTol.Models;
namespace MolTol.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    Task<Category?> GetByIdAsync(
        int categoryId,
        CancellationToken cancellationToken = default);

    Task<int> AddAsync(
        Category category,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Category category,
        CancellationToken cancellationToken = default);

    Task<bool> SetActiveAsync(
        int categoryId,
        bool isActive,
        CancellationToken cancellationToken = default);
}
