using Application.Queues;
using DropSort.Core.Enums;
using DropSort.Core.Interfaces;
using DropSort.Core.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class QueueProcessingService
{
    private readonly InMemoryQueues _queues;
    private readonly IFileMover _mover;
    private readonly ISystemIdleChecker _idle;
    private readonly ICategoryPathResolver _resolver;
    private readonly IFileTaskRepository _repo;
    private readonly ILogRepository _logRepo;
    private readonly ILogger<QueueProcessingService> _logger;
    private volatile bool _forceLowProcessing;

    public QueueProcessingService(
        InMemoryQueues queues,
        IFileMover mover,
        ISystemIdleChecker idle,
        ICategoryPathResolver categoryResolver,
        IFileTaskRepository taskRepository,
        ILogRepository logRepository,
        ILogger<QueueProcessingService> logger)
    {
        _queues = queues;
        _mover = mover;
        _idle = idle;
        _resolver = categoryResolver;
        _repo = taskRepository;
        _logRepo = logRepository;
        _logger = logger;
    }
    
    public void ForceProcessLow()
    {
        _forceLowProcessing = true;
    }

    public async Task ProcessOnceAsync(CancellationToken ct)
    {
        if (_queues.High.TryDequeue(out var task))
        {
            await ProcessTaskAsync(task, ct);
            return;
        }

        if ((_forceLowProcessing || _idle.IsIdle()) &&
            _queues.Low.TryDequeue(out var low))
        {
            await ProcessTaskAsync(low, ct);
            
            if (_queues.Low.IsEmpty)
            {
                _forceLowProcessing = false;
            }
        }
    }

    private async Task ProcessTaskAsync(FileTask task, CancellationToken ct)
    {
        // giữ source path TRƯỚC khi move
        var sourcePath = task.FullPath;

        try
        {
            // ===== PROCESSING =====
            task.Status = FileTaskStatus.Processing;
            await _repo.UpdateAsync(task);

            var targetPath = _resolver.ResolveTargetPath(task);
            await _mover.MoveAsync(task, targetPath, ct);

            // ===== LOG SUCCESS =====
            await _logRepo.AddAsync(new LogEntry
            {
                Level = "INFO",
                Message = "File moved successfully",
                FileName = task.FileName,
                SourcePath = sourcePath,
                TargetPath = targetPath,
                CreatedAt = DateTime.UtcNow
            });

            // ===== DELETE TASK =====
            await _repo.DeleteAsync(task.Id);
        }
        catch (Exception ex)
        {
            // ===== LOG ERROR =====
            await _logRepo.AddAsync(new LogEntry
            {
                Level = "ERROR",
                Message = ex.Message,
                FileName = task.FileName,
                SourcePath = sourcePath,
                CreatedAt = DateTime.UtcNow
            });

            task.Status = FileTaskStatus.Failed;
            await _repo.UpdateAsync(task);

            _logger.LogError(ex,
                "Failed processing {file}", task.FileName);
        }
    }
}
