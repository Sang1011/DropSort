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
    private readonly ILogService _log;
    private volatile bool _forceLowProcessing;
    private readonly IDiskSpaceChecker _disk;
    private bool _waitingLogged;

    public QueueProcessingService(
        InMemoryQueues queues,
        IFileMover mover,
        ISystemIdleChecker idle,
        ICategoryPathResolver categoryResolver,
        IFileTaskRepository taskRepository,
        ILogService logService,
        IDiskSpaceChecker disk)
    {
        _queues = queues;
        _mover = mover;
        _idle = idle;
        _resolver = categoryResolver;
        _repo = taskRepository;
        _log = logService;
        _disk = disk;
    }

    
    public void ForceProcessLow()
    {
        _forceLowProcessing = true;
    }

    public async Task ProcessOnceAsync(CancellationToken ct)
    {
        if (_queues.High.TryDequeue(out var high))
        {
            await ProcessTaskAsync(high, ct);
            return;
        }

        if (_queues.Low.IsEmpty)
            return;
        
        if (!_forceLowProcessing)
        {
            await Task.Delay(200, ct);

            if (!_idle.IsIdle())
            {
                await LogWaitingForIdleOnce();
                return;
            }
        }

        if (_queues.Low.TryDequeue(out var low))
        {
            _waitingLogged = false;
            await ProcessTaskAsync(low, ct);
        }
    }

    private async Task ProcessTaskAsync(FileTask task, CancellationToken ct)
    {
        var sourcePath = task.FullPath;

        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
        {
            await _log.InfoAsync(
                LogEvent.SourceFileMissing,
                task.FileName,
                sourcePath
            );

            await _repo.DeleteAsync(task.Id);
            await LogRemainingTasksAsync();
            return;
        }

        try
        {
            task.Status = FileTaskStatus.Processing;
            await _repo.UpdateAsync(task);

            await _log.InfoAsync(
                LogEvent.ProcessingStarted,
                task.FileName,
                sourcePath
            );

            var targetPath = task.TargetPath
                             ?? throw new InvalidOperationException("TargetPath is not set");
            if (_disk.IsDiskFull(targetPath))
            {
                await _log.ErrorAsync(
                    LogEvent.DiskFull,
                    new IOException("Target disk is full"),
                    task.FileName,
                    sourcePath,
                    targetPath
                );

                task.Status = FileTaskStatus.Failed;
                await _repo.UpdateAsync(task);
                return;
            }
            await _mover.MoveAsync(task, targetPath, ct);

            await _log.InfoAsync(
                LogEvent.FileMoved,
                task.FileName,
                sourcePath,
                targetPath);

            await _log.InfoAsync(
                LogEvent.ProcessingCompleted,
                task.FileName,
                sourcePath,
                targetPath
            );

            await _repo.DeleteAsync(task.Id);
            await LogRemainingTasksAsync();
        }
        catch (Exception ex)
        {
            await _log.ErrorAsync(
                LogEvent.FileMoveFailed,
                ex,
                task.FileName,
                sourcePath);

            task.Status = FileTaskStatus.Failed;
            await _repo.UpdateAsync(task);
        }
    }

    private async Task LogWaitingForIdleOnce()
    {
        if (_waitingLogged)
            return;

        if (!_queues.Low.TryPeek(out var peek))
            return;

        await _log.InfoAsync(
            LogEvent.WaitingForIdle,
            peek.FileName,
            peek.FullPath
        );

        _waitingLogged = true;
    }

    private async Task LogRemainingTasksAsync()
    {
        var remaining =
            _queues.High.Count +
            _queues.Low.Count;

        if (remaining > 0)
        {
            await _log.InfoAsync(
                LogEvent.RemainingTasks,
                $"Remaining tasks: {remaining}"
            );
        }
        else
        {
            await _log.InfoAsync(
                LogEvent.AllTasksCompleted,
                "All files have been processed"
            );
        }
    }


}
