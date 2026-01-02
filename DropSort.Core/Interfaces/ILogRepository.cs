using DropSort.Core.Models;

namespace DropSort.Core.Interfaces;

public interface ILogRepository
{
    Task AddAsync(LogEntry entry);

    Task<IReadOnlyList<LogEntry>> GetLatestAsync(int limit = 100);

    Task DeleteAllAsync();
}