namespace Infrastructure.Persistence.Scripts.Strings;

internal static class FileTaskSql
{
    public const string Insert = """
                                     INSERT INTO file_tasks (
                                         id, full_path, file_name, extension,
                                         size_in_bytes, source_drive,
                                         category, target_path,
                                         status, created_at
                                     )
                                     VALUES (
                                         @Id, @FullPath, @FileName, @Extension,
                                         @SizeInBytes, @SourceDrive,
                                         @Category, @TargetPath,
                                         @Status, @CreatedAt
                                     );
                                 """;

    public const string Update = """
                                     UPDATE file_tasks
                                     SET
                                         status = @Status,
                                         target_path = @TargetPath
                                     WHERE id = @Id;
                                 """;

    public const string GetByPath = """
                                        SELECT *
                                        FROM file_tasks
                                        WHERE full_path = @fullPath
                                        LIMIT 1;
                                    """;

    public const string GetPending = """
                                         SELECT *
                                         FROM file_tasks
                                         WHERE status = @Pending
                                            OR status = @Processing;
                                     """;

    public const string DeleteById = """
                                         DELETE FROM file_tasks
                                         WHERE id = @id;
                                     """;
}