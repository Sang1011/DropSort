using Dapper;
using DropSort.Core.Enums;
using DropSort.Core.Interfaces;
using DropSort.Core.Models;
using Infrastructure.Persistence.Sqlite;
using Infrastructure.Persistence.Scripts.Strings;

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
        if (string.IsNullOrWhiteSpace(task.Id))
            throw new ArgumentException("FileTask.Id is required");

        if (string.IsNullOrWhiteSpace(task.FullPath))
            throw new ArgumentException("FileTask.FullPath is required");

        using var conn = _connectionFactory.Create();

        var affected = await conn.ExecuteAsync(
            FileTaskSql.Insert,
            task
        );

        if (affected != 1)
            throw new InvalidOperationException(
                $"Insert FileTask failed (affected rows = {affected})"
            );
    }

    public async Task UpdateAsync(FileTask task)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(FileTaskSql.Update, task);
    }

    public async Task<FileTask?> GetByPathAsync(string fullPath)
    {
        using var conn = _connectionFactory.Create();
        return await conn.QueryFirstOrDefaultAsync<FileTask>(
            FileTaskSql.GetByPath,
            new { fullPath }
        );
    }

    public async Task<IReadOnlyList<FileTask>> GetPendingAsync()
    {
        using var conn = _connectionFactory.Create();
        return (await conn.QueryAsync<FileTask>(
            FileTaskSql.GetPending,
            new
            {
                Pending = FileTaskStatus.Pending,
                Processing = FileTaskStatus.Processing
            }
        )).ToList();
    }

    public async Task DeleteAsync(string id)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(FileTaskSql.DeleteById, new { id });
    }
}
