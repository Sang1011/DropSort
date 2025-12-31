namespace Infrastructure.Persistence.Scripts.Strings;

internal static class LogSql
{
    public const string Insert = """
                                     INSERT INTO logs
                                     (level, message, file_name, source_path, target_path, created_at)
                                     VALUES
                                     (@Level, @Message, @FileName, @SourcePath, @TargetPath, @CreatedAt)
                                 """;
}