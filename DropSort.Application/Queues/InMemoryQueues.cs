using System.Collections.Concurrent;
using DropSort.Core.Models;

namespace Application.Queues;

public class InMemoryQueues
{
    public ConcurrentQueue<FileItem> High { get; } = new();
    public ConcurrentQueue<FileItem> Low { get; } = new();
}