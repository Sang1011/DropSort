using Microsoft.Data.Sqlite;

namespace Infrastructure.Persistence.Sqlite;

public class SqliteConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }

    public SqliteConnection Create()
        => new SqliteConnection(_connectionString);
}