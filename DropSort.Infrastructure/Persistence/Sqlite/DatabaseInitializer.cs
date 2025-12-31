using Dapper;

namespace Infrastructure.Persistence.Sqlite;

public class DatabaseInitializer
{
    private readonly SqliteConnectionFactory _factory;

    public DatabaseInitializer(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public void Initialize()
    {
        var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
            "Persistence", "Scripts", "Init.sql");

        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"Không tìm thấy script khởi tạo tại: {scriptPath}");
        }

        var sql = File.ReadAllText(scriptPath);

        using var conn = _factory.Create();
        conn.Open();

        conn.Execute(sql);
    }
}