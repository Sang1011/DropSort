using Dapper;
using DropSort.Core.Interfaces;
using DropSort.Core.Models;
using Infrastructure.Persistence.Sqlite;
using Infrastructure.Persistence.Scripts.Strings;

namespace Infrastructure.Persistence.Repositories;

public class LogRepository : ILogRepository
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public LogRepository(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(LogEntry entry)
    {
        using var conn = _connectionFactory.Create();

        await conn.ExecuteAsync(
            LogSql.Insert,
            new
            {
                entry.Level,
                entry.Message,
                entry.FileName,
                entry.SourcePath,
                entry.TargetPath,
                CreatedAt = entry.CreatedAt.ToString("O")
            }
        );
    }

    public async Task<IReadOnlyList<LogEntry>> GetLatestAsync(int limit = 100)
    {
        using var conn = _connectionFactory.Create();

        return (await conn.QueryAsync<LogEntry>(
            LogSql.GetLatest,
            new { limit }
        )).ToList();
    }

    public async Task DeleteAllAsync()
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(LogSql.DeleteAll);
    }
}