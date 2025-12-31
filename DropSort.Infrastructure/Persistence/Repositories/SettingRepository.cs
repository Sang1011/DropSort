using Dapper;
using DropSort.Core.Interfaces;
using Infrastructure.Persistence.Sqlite;
using Infrastructure.Persistence.Scripts.Strings;

namespace Infrastructure.Persistence.Repositories;

public class SettingRepository : ISettingRepository
{
    private readonly SqliteConnectionFactory _factory;

    public SettingRepository(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public void Set(string key, string value)
    {
        using var conn = _factory.Create();
        conn.Execute(SettingSql.Upsert, new { key, value });
    }

    public string? Get(string key)
    {
        using var conn = _factory.Create();
        return conn.QueryFirstOrDefault<string>(
            SettingSql.GetByKey,
            new { key }
        );
    }
}