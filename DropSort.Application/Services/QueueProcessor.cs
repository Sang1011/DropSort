using DropSort.Core.Interfaces;
using DropSort.Core.Models;
using Application.Queues;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class QueueProcessor : IQueueProcessor
{
    private readonly InMemoryQueues _queues;
    private readonly ILogger<QueueProcessor> _logger;

    public QueueProcessor(
        InMemoryQueues queues,
        ILogger<QueueProcessor> logger)
    {
        _queues = queues;
        _logger = logger;
    }

    public void EnqueueHigh(FileItem file)
    {
        _queues.High.Enqueue(file);
        _logger.LogInformation(
            "[QUEUE:HIGH] {file}", file.FileName);
    }

    public void EnqueueLow(FileItem file)
    {
        _queues.Low.Enqueue(file);
        _logger.LogInformation(
            "[QUEUE:LOW] {file}", file.FileName);
    }
}