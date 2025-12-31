using DropSort.Core.Models;

namespace DropSort.Core.Interfaces;

public interface IQueueProcessor
{
    void EnqueueHigh(FileItem file);
    void EnqueueLow(FileItem file);
}