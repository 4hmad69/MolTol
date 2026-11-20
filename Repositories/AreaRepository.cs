
using Dapper;
using MolTol.Data;
using MolTol.Interfaces;
using MolTol.Models;

namespace MolTol.Repositories;

public class AreaRepository : IAreaRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public AreaRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Area>> GetAllAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT AreaId, Name, IsActive
            FROM Areas
            WHERE (@ActiveOnly = 0 OR IsActive = 1)
            ORDER BY Name;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var areas = await connection.QueryAsync<Area>(
            new CommandDefinition(
                sql,
                new { ActiveOnly = activeOnly },
                cancellationToken: cancellationToken));

        return areas.ToList();
    }

    public async Task<Area?> GetByIdAsync(
        int areaId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT AreaId, Name, IsActive
            FROM Areas
            WHERE AreaId = @AreaId;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Area>(
            new CommandDefinition(
                sql,
                new { AreaId = areaId },
                cancellationToken: cancellationToken));
    }

    public async Task<int> AddAsync(
        Area area,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Areas (Name, IsActive)
            VALUES (@Name, @IsActive);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                sql,
                area,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(
        Area area,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Areas
            SET Name = @Name
            WHERE AreaId = @AreaId;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                area,
                cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<bool> SetActiveAsync(
        int areaId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Areas
            SET IsActive = @IsActive
            WHERE AreaId = @AreaId;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { AreaId = areaId, IsActive = isActive },
                cancellationToken: cancellationToken));

        return affectedRows > 0;
    }
}
