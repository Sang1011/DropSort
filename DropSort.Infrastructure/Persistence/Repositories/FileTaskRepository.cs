using Dapper;
using DropSort.Core.Enums;
using DropSort.Core.Interfaces;
using DropSort.Core.Models;
using Infrastructure.Persistence.Sqlite;

namespace Infrastructure.Persistence.Repositories;

public class FileTaskRepository : IFileTaskRepository
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public FileTaskRepository(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(FileTask task)
    {
        const string sql = """
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

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql, task);
    }

    public async Task UpdateAsync(FileTask task)
    {
        const string sql = """
        UPDATE file_tasks
        SET
            status = @Status,
            target_path = @TargetPath,
            completed_at = @CompletedAt
        WHERE id = @Id;
        """;

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql, task);
    }

    public async Task<FileTask?> GetByPathAsync(string fullPath)
    {
        const string sql = """
        SELECT * FROM file_tasks
        WHERE full_path = @fullPath
        LIMIT 1;
        """;

        using var conn = _connectionFactory.Create();
        return await conn.QueryFirstOrDefaultAsync<FileTask>(sql, new { fullPath });
    }

    public async Task<IReadOnlyList<FileTask>> GetPendingAsync()
    {
        const string sql = """
        SELECT * FROM file_tasks
        WHERE status IN (@Pending, @Processing);
        """;

        using var conn = _connectionFactory.Create();
        return (await conn.QueryAsync<FileTask>(sql, new
        {
            Pending = FileTaskStatus.Pending,
            Processing = FileTaskStatus.Processing
        })).ToList();
    }
    
    public async Task DeleteAsync(Guid id)
    {
        const string sql = """
                           DELETE FROM file_tasks
                           WHERE id = @id;
                           """;

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql, new { id });
    }
}