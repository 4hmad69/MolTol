
using Dapper;
using MolTol.Data;
using MolTol.Interfaces;
using MolTol.Models;

namespace MolTol.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public ProductRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(
        int? categoryId = null,
        string? searchTerm = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT ProductId, Name, CategoryId, Unit, IsActive
            FROM Products
            WHERE (@CategoryId IS NULL OR CategoryId = @CategoryId)
              AND (@SearchTerm IS NULL OR Name LIKE '%' + @SearchTerm + '%')
              AND (@ActiveOnly = 0 OR IsActive = 1)
            ORDER BY Name;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var products = await connection.QueryAsync<Product>(
            new CommandDefinition(
                sql,
                new
                {
                    CategoryId = categoryId,
                    SearchTerm = string.IsNullOrWhiteSpace(searchTerm)
                        ? null
                        : searchTerm.Trim(),
                    ActiveOnly = activeOnly
                },
                cancellationToken: cancellationToken));

        return products.ToList();
    }

    public async Task<Product?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT ProductId, Name, CategoryId, Unit, IsActive
            FROM Products
            WHERE ProductId = @ProductId;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Product>(
            new CommandDefinition(
                sql,
                new { ProductId = productId },
                cancellationToken: cancellationToken));
    }

    public async Task<int> AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Products (Name, CategoryId, Unit, IsActive)
            VALUES (@Name, @CategoryId, @Unit, @IsActive);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                sql,
                product,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Products
            SET Name = @Name,
                CategoryId = @CategoryId,
                Unit = @Unit
            WHERE ProductId = @ProductId;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                product,
                cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<bool> SetActiveAsync(
        int productId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Products
            SET IsActive = @IsActive
            WHERE ProductId = @ProductId;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { ProductId = productId, IsActive = isActive },
                cancellationToken: cancellationToken));

        return affectedRows > 0;
    }
}
