using Application.Queues;
using DropSort.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class QueueProcessingService
{
    private readonly InMemoryQueues _queues;
    private readonly IFileMover _mover;
    private readonly ISystemIdleChecker _idle;
    private readonly ICategoryPathResolver _categoryResolver;
    private readonly ILogger<QueueProcessingService> _logger;

    public QueueProcessingService(
        InMemoryQueues queues,
        IFileMover mover,
        ISystemIdleChecker idle,
        ICategoryPathResolver categoryResolver,
        ILogger<QueueProcessingService> logger)
    {
        _queues = queues;
        _mover = mover;
        _idle = idle;
        _categoryResolver = categoryResolver;
        _logger = logger;
    }

    public async Task ProcessOnceAsync(CancellationToken ct)
    {
        // ===== HIGH PRIORITY =====
        if (_queues.High.TryDequeue(out var high))
        {
            _logger.LogInformation("[PROCESS:HIGH] {file}", high.FileName);

            var targetPath = _categoryResolver.ResolveTargetPath(high);
            await _mover.MoveAsync(high, targetPath, ct);

            return;
        }

        // ===== LOW PRIORITY =====
        if (_idle.IsIdle() &&
            _queues.Low.TryDequeue(out var low))
        {
            _logger.LogInformation("[PROCESS:LOW] {file}", low.FileName);

            var targetPath = _categoryResolver.ResolveTargetPath(low);
            await _mover.MoveAsync(low, targetPath, ct);
        }
    }
}