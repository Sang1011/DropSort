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
}
