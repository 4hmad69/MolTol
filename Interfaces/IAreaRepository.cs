
using MolTol.Models;
namespace MolTol.Interfaces;
public interface IAreaRepository
{
    Task<IReadOnlyList<Area>> GetAllAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    Task<Area?> GetByIdAsync(
        int areaId,
        CancellationToken cancellationToken = default);

    Task<int> AddAsync(
        Area area,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Area area,
        CancellationToken cancellationToken = default);

    Task<bool> SetActiveAsync(
        int areaId,
        bool isActive,
        CancellationToken cancellationToken = default);
}
