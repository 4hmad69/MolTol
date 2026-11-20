
using MolTol.Models;
namespace MolTol.Interfaces;

public interface IShopRepository
{
    Task<IReadOnlyList<Shop>> GetAllAsync(
        int? areaId = null,
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<Shop?> GetByIdAsync(
        int shopId,
        CancellationToken cancellationToken = default);

    Task<int> AddAsync(
        Shop shop,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Shop shop,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateStatusAsync(
        int shopId,
        string status,
        CancellationToken cancellationToken = default);
}
