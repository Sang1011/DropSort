using DropSort.Core.Models;

namespace DropSort.Core.Interfaces;

public interface IQueueProcessor
{
    void EnqueueHigh(FileTask task);
    void EnqueueLow(FileTask task);
}