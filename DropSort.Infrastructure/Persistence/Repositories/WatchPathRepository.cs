using Dapper;
using DropSort.Core.Interfaces;
using Infrastructure.Persistence.Sqlite;

namespace Infrastructure.Persistence.Repositories;

public class WatchPathRepository : IWatchPathRepository
{
    private readonly SqliteConnectionFactory _factory;

    public WatchPathRepository(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<string> GetEnabledPaths()
    {
        using var conn = _factory.Create();
        return conn.Query<string>(
            "SELECT path FROM watch_paths WHERE enabled = 1"
        ).ToList();
    }

    public async Task AddAsync(string path, bool enabled = true)
    {
        const string sql = """
                           INSERT OR IGNORE INTO watch_paths (path, enabled)
                           VALUES (@path, @enabled);
                           """;

        using var conn = _factory.Create();
        await conn.ExecuteAsync(sql, new
        {
            path,
            enabled = enabled ? 1 : 0
        });
    }

    public async Task DeleteAsync(string path)
    {
        const string sql = """
                           DELETE FROM watch_paths
                           WHERE path = @path;
                           """;

        using var conn = _factory.Create();
        await conn.ExecuteAsync(sql, new { path });
    }

    public async Task SetEnabledAsync(string path, bool enabled)
    {
        const string sql = """
                           UPDATE watch_paths
                           SET enabled = @enabled
                           WHERE path = @path;
                           """;

        using var conn = _factory.Create();
        await conn.ExecuteAsync(sql, new
        {
            path,
            enabled = enabled ? 1 : 0
        });
    }
}