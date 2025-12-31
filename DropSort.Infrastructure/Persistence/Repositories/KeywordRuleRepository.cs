using Dapper;
using DropSort.Core.Interfaces;
using DropSort.Core.Models;
using Infrastructure.Persistence.Sqlite;

namespace Infrastructure.Persistence.Repositories;

public class KeywordRuleRepository : IKeywordRuleRepository
{
    private readonly SqliteConnectionFactory _factory;

    public KeywordRuleRepository(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<KeywordRule> GetEnabledRules()
    {
        using var conn = _factory.Create();

        var rows = conn.Query<KeywordRuleRow>(
            """
            SELECT
                id,
                keyword,
                extensions,
                target_folder,
                priority,
                enabled
            FROM keyword_rules
            WHERE enabled = 1
            """
        );

        return rows.Select(r => new KeywordRule
        {
            Id = r.Id,
            Keyword = r.Keyword,
            Extensions = string.IsNullOrWhiteSpace(r.Extensions)
                ? null
                : r.Extensions
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim().ToLowerInvariant())
                    .ToList(),
            TargetFolder = r.TargetFolder,
            Priority = r.Priority,
            Enabled = r.Enabled == 1
        }).ToList();
    }

    private sealed class KeywordRuleRow
    {
        public int Id { get; init; }
        public string Keyword { get; init; } = "";
        public string? Extensions { get; init; }
        public string TargetFolder { get; init; } = "";
        public int Priority { get; init; }
        public int Enabled { get; init; }
    }
}