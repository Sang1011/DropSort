using DropSort.Core.Enums;

namespace DropSort.Core.Interfaces;

public interface ILogService
{
    Task InfoAsync(
        LogEvent evt,
        string? fileName = null,
        string? sourcePath = null,
        string? targetPath = null);

    Task ErrorAsync(
        LogEvent evt,
        Exception ex,
        string? fileName = null,
        string? sourcePath = null,
        string? targetPath = null);
}