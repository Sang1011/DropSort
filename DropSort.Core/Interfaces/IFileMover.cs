using DropSort.Core.Models;

namespace DropSort.Core.Interfaces;

public interface IFileMover
{
    Task MoveAsync(FileItem file, string targetPath, CancellationToken ct);
}