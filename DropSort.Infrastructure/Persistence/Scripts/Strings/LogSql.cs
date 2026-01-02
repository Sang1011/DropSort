namespace Infrastructure.Persistence.Scripts.Strings;

internal static class LogSql
{
    public const string Insert = """
                                     INSERT INTO logs
                                     (level, message, file_name, source_path, target_path, created_at)
                                     VALUES
                                     (@Level, @Message, @FileName, @SourcePath, @TargetPath, @CreatedAt)
                                 """;

    public const string GetLatest = """
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
                                        LIMIT @limit
                                    """;

    public const string DeleteAll = """
                                        DELETE FROM logs
                                    """;
}