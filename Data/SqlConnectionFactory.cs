using Microsoft.Data.SqlClient;

namespace MolTol.Data;

public class SqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ApplicationDbContext")
            ?? throw new InvalidOperationException(
                "Database connection string not found.");
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
