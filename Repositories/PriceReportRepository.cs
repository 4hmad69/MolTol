
using Dapper;
using MolTol.Data;
using MolTol.Interfaces;
using MolTol.Models;

namespace MolTol.Repositories;

public class PriceReportRepository : IPriceReportRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public PriceReportRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<PriceReport>> GetAllAsync(
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT ReportId, ProductId, ShopId, UserId,
                   Price, ReportDate, Status
            FROM PriceReports
            WHERE (@Status IS NULL OR Status = @Status)
            ORDER BY ReportDate DESC;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var reports = await connection.QueryAsync<PriceReport>(
            new CommandDefinition(
                sql,
                new
                {
                    Status = string.IsNullOrWhiteSpace(status)
                        ? null
                        : status.Trim()
                },
                cancellationToken: cancellationToken));

        return reports.ToList();
    }

    public async Task<IReadOnlyList<PriceReport>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT ReportId, ProductId, ShopId, UserId,
                   Price, ReportDate, Status
            FROM PriceReports
            WHERE UserId = @UserId
            ORDER BY ReportDate DESC;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var reports = await connection.QueryAsync<PriceReport>(
            new CommandDefinition(
                sql,
                new { UserId = userId },
                cancellationToken: cancellationToken));

        return reports.ToList();
    }

    public async Task<PriceReport?> GetByIdAsync(
        int reportId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT ReportId, ProductId, ShopId, UserId,
                   Price, ReportDate, Status
            FROM PriceReports
            WHERE ReportId = @ReportId;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<PriceReport>(
            new CommandDefinition(
                sql,
                new { ReportId = reportId },
                cancellationToken: cancellationToken));
    }

    public async Task<int> AddAsync(
        PriceReport report,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO PriceReports
                (ProductId, ShopId, UserId, Price, Status)
            VALUES
                (@ProductId, @ShopId, @UserId, @Price, 'Pending');

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                sql,
                new
                {
                    report.ProductId,
                    report.ShopId,
                    report.UserId,
                    report.Price
                },
                cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateStatusAsync(
        int reportId,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (status is not ("Approved" or "Rejected"))
        {
            throw new ArgumentException(
                "Status must be Approved or Rejected.",
                nameof(status));
        }

        const string sql = """
            UPDATE PriceReports
            SET Status = @Status
            WHERE ReportId = @ReportId
              AND Status = 'Pending';
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { ReportId = reportId, Status = status },
                cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<int> CountAsync(
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM PriceReports
            WHERE (@Status IS NULL OR Status = @Status);
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                sql,
                new
                {
                    Status = string.IsNullOrWhiteSpace(status)
                        ? null
                        : status.Trim()
                },
                cancellationToken: cancellationToken));
    }

    public async Task<int> CountByUserIdAsync(
        string userId,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM PriceReports
            WHERE UserId = @UserId
              AND (@Status IS NULL OR Status = @Status);
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                sql,
                new
                {
                    UserId = userId,
                    Status = string.IsNullOrWhiteSpace(status)
                        ? null
                        : status.Trim()
                },
                cancellationToken: cancellationToken));
    }
}
