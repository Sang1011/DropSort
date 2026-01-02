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

        public void EnqueueHigh(FileTask task)
        {
            _queues.High.Enqueue(task);
            _logger.LogInformation(
                "[QUEUE:HIGH] {file}", task.FileName);
        }

        public void EnqueueLow(FileTask task)
        {
            _queues.Low.Enqueue(task);
            _logger.LogInformation(
                "[QUEUE:LOW] {file}", task.FileName);
        }
}