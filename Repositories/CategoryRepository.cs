using Dapper;
using MolTol.Data;
using MolTol.Interfaces;
using MolTol.Models;

namespace MolTol.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public CategoryRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT CategoryId, Name, Description, IsActive
            FROM Categories
            WHERE (@ActiveOnly = 0 OR IsActive = 1)
            ORDER BY Name;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var categories = await connection.QueryAsync<Category>(
            new CommandDefinition(
                sql,
                new { ActiveOnly = activeOnly },
                cancellationToken: cancellationToken));

        return categories.ToList();
    }

    public async Task<Category?> GetByIdAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT CategoryId, Name, Description, IsActive
            FROM Categories
            WHERE CategoryId = @CategoryId;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Category>(
            new CommandDefinition(
                sql,
                new { CategoryId = categoryId },
                cancellationToken: cancellationToken));
    }

    public async Task<int> AddAsync(
        Category category,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Categories (Name, Description, IsActive)
            VALUES (@Name, @Description, @IsActive);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                sql,
                category,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(
        Category category,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Categories
            SET Name = @Name,
                Description = @Description
            WHERE CategoryId = @CategoryId;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                category,
                cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<bool> SetActiveAsync(
        int categoryId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Categories
            SET IsActive = @IsActive
            WHERE CategoryId = @CategoryId;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { CategoryId = categoryId, IsActive = isActive },
                cancellationToken: cancellationToken));

        return affectedRows > 0;
    }
}
