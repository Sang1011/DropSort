using Dapper;
using DropSort.Core.Interfaces;
using DropSort.Core.Models;
using Infrastructure.Persistence.Sqlite;
using Infrastructure.Persistence.Scripts.Strings;


namespace Infrastructure.Persistence.Repositories;

public class LogRepository : ILogRepository
{
    private readonly SqliteConnectionFactory _factory;

    public LogRepository(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public void Write(LogEntry entry)
    {
        using var conn = _factory.Create();
        conn.Execute(LogSql.Insert, entry);
    }
}