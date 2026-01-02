namespace Infrastructure.Persistence.Scripts.Strings;

internal static class WatchPathSql
{
    public const string GetEnabled = """
                                         SELECT path
                                         FROM watch_paths
                                         WHERE enabled = 1;
                                     """;

    public const string Insert = """
                                     INSERT OR IGNORE INTO watch_paths (path, enabled)
                                     VALUES (@path, @enabled);
                                 """;

    public const string Delete = """
                                     DELETE FROM watch_paths
                                     WHERE path = @path;
                                 """;

    public const string SetEnabled = """
                                         UPDATE watch_paths
                                         SET enabled = @enabled
                                         WHERE path = @path;
                                     """;
}