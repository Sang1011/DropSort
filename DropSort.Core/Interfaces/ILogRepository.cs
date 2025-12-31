using DropSort.Core.Models;

namespace DropSort.Core.Interfaces;

public interface ILogRepository
{
    void Write(LogEntry entry);
}