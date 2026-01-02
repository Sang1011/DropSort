using Dapper;
using DropSort.Core.Interfaces;
using DropSort.Core.Models;
using Infrastructure.Persistence.Sqlite;

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
        const string sql = """
                           INSERT INTO logs (
                               level,
                               message,
                               file_name,
                               source_path,
                               target_path,
                               created_at
                           )
                           VALUES (
                               @Level,
                               @Message,
                               @FileName,
                               @SourcePath,
                               @TargetPath,
                               @CreatedAt
                           );
                           """;

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql, new
        {
            entry.Level,
            entry.Message,
            entry.FileName,
            entry.SourcePath,
            entry.TargetPath,
            CreatedAt = entry.CreatedAt.ToString("O")
        });
    }

    public async Task<IReadOnlyList<LogEntry>> GetLatestAsync(int limit = 100)
    {
        const string sql = """
                           SELECT
                               id,
                               level,
                               message,
                               file_name   AS FileName,
                               source_path AS SourcePath,
                               target_path AS TargetPath,
                               created_at  AS CreatedAt
                           FROM logs
                           ORDER BY id DESC
                           LIMIT @limit;
                           """;

        using var conn = _connectionFactory.Create();
        return (await conn.QueryAsync<LogEntry>(sql, new { limit })).ToList();
    }

    public async Task DeleteAllAsync()
    {
        const string sql = """
                           DELETE FROM logs;
                           """;

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql);
    }
}