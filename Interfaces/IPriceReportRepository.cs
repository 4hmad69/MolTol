
using MolTol.Models;

namespace MolTol.Interfaces;

public interface IPriceReportRepository
{
    Task<IReadOnlyList<PriceReport>> GetAllAsync(
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PriceReport>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<PriceReport?> GetByIdAsync(
        int reportId,
        CancellationToken cancellationToken = default);

    Task<int> AddAsync(
        PriceReport report,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateStatusAsync(
        int reportId,
        string status,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<int> CountByUserIdAsync(
        string userId,
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PriceComparisonItem>> GetApprovedComparisonsAsync(
        int? categoryId = null,
        int? areaId = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
}
