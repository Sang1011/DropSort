using DropSort.Core.Enums;
using DropSort.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Logging;

public class LogService : ILogService
{
    private readonly ILogRepository _repo;
    private readonly ILogger<LogService> _logger;

    public LogService(
        ILogRepository repo,
        ILogger<LogService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public Task InfoAsync(
        LogEvent evt,
        string? fileName = null,
        string? sourcePath = null,
        string? targetPath = null)
    {
        var message = LogMessageResolver.ResolveMessage(evt);

        _logger.LogInformation(message);

        return _repo.AddAsync(LogEntryFactory.CreateEntry(
            "INFO", message, fileName, sourcePath, targetPath));
    }

    public Task ErrorAsync(
        LogEvent evt,
        Exception ex,
        string? fileName = null,
        string? sourcePath = null,
        string? targetPath = null)
    {
        var message = LogMessageResolver.ResolveMessage(evt);

        _logger.LogError(ex, message);

        return _repo.AddAsync(LogEntryFactory.CreateEntry(
            "ERROR", message, fileName, sourcePath, targetPath));
    }
}

