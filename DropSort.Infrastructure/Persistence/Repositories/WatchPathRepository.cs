using Dapper;
using DropSort.Core.Interfaces;
using Infrastructure.Persistence.Sqlite;
using Infrastructure.Persistence.Scripts.Strings;

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
        return conn.Query<string>(WatchPathSql.GetEnabled).ToList();
    }

    public async Task AddAsync(string path, bool enabled = true)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            WatchPathSql.Insert,
            new
            {
                path,
                enabled = enabled ? 1 : 0
            }
        );
    }

    public async Task DeleteAsync(string path)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            WatchPathSql.Delete,
            new { path }
        );
    }

    public async Task SetEnabledAsync(string path, bool enabled)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            WatchPathSql.SetEnabled,
            new
            {
                path,
                enabled = enabled ? 1 : 0
            }
        );
    }
}