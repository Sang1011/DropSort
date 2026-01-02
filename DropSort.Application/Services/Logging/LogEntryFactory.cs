using DropSort.Core.Models;

namespace Application.Services.Logging;

public static class LogEntryFactory
{
    public static LogEntry CreateEntry(
        string level,
        string message,
        string? fileName,
        string? sourcePath,
        string? targetPath)
    {
        return new LogEntry
        {
            Level = level,
            Message = message,
            FileName = fileName,
            SourcePath = sourcePath,
            TargetPath = targetPath,
            CreatedAt = DateTime.UtcNow
        };
    }
}