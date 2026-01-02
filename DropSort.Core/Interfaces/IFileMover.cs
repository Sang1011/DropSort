using DropSort.Core.Models;

namespace DropSort.Core.Interfaces;

public interface IFileMover
{
    Task MoveAsync(FileTask task, string targetPath, CancellationToken ct);
}