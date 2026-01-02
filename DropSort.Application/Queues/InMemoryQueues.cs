using System.Collections.Concurrent;
using DropSort.Core.Models;

namespace Application.Queues;

public class InMemoryQueues
{
    public ConcurrentQueue<FileTask> High { get; } = new();
    public ConcurrentQueue<FileTask> Low { get; } = new();
}