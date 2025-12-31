namespace Infrastructure.Persistence.Scripts.Strings;

internal static class SettingSql
{
    public const string Upsert = """
                                     INSERT INTO settings(key, value)
                                     VALUES (@key, @value)
                                     ON CONFLICT(key)
                                     DO UPDATE SET value = excluded.value
                                 """;

    public const string GetByKey = """
                                       SELECT value FROM settings WHERE key = @key
                                   """;
}