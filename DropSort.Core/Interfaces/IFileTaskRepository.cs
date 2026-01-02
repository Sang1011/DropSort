using DropSort.Core.Models;

namespace DropSort.Core.Interfaces;

public interface IFileTaskRepository
{
    Task AddAsync(FileTask task);
    Task UpdateAsync(FileTask task);

    Task<FileTask?> GetByPathAsync(string fullPath);
    Task<IReadOnlyList<FileTask>> GetPendingAsync();

    Task DeleteAsync(string id);
}