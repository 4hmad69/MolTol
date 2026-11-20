
using Dapper;
using MolTol.Data;
using MolTol.Interfaces;
using MolTol.Models;

namespace MolTol.Repositories;

public class ShopRepository : IShopRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public ShopRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Shop>> GetAllAsync(
        int? areaId = null,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT ShopId, Name, AreaId, Status, CreatedAt, SuggestedByUserId
            FROM Shops
            WHERE (@AreaId IS NULL OR AreaId = @AreaId)
              AND (@Status IS NULL OR Status = @Status)
            ORDER BY CreatedAt DESC;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var shops = await connection.QueryAsync<Shop>(
            new CommandDefinition(
                sql,
                new
                {
                    AreaId = areaId,
                    Status = string.IsNullOrWhiteSpace(status)
                        ? null
                        : status.Trim()
                },
                cancellationToken: cancellationToken));

        return shops.ToList();
    }

    public async Task<Shop?> GetByIdAsync(
        int shopId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT ShopId, Name, AreaId, Status, CreatedAt, SuggestedByUserId
            FROM Shops
            WHERE ShopId = @ShopId;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Shop>(
            new CommandDefinition(
                sql,
                new { ShopId = shopId },
                cancellationToken: cancellationToken));
    }

    public async Task<int> AddAsync(
        Shop shop,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Shops (Name, AreaId, Status, SuggestedByUserId)
            VALUES (@Name, @AreaId, 'Pending', @SuggestedByUserId);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                sql,
                new
                {
                    shop.Name,
                    shop.AreaId,
                    shop.SuggestedByUserId
                },
                cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(
        Shop shop,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Shops
            SET Name = @Name,
                AreaId = @AreaId
            WHERE ShopId = @ShopId;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                shop,
                cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<bool> UpdateStatusAsync(
    int shopId,
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
        UPDATE Shops
        SET Status = @Status
        WHERE ShopId = @ShopId
          AND Status = 'Pending';
        """;

        await using var connection = _connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { ShopId = shopId, Status = status },
                cancellationToken: cancellationToken));

        return affectedRows > 0;
    }
}
