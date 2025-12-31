using DropSort.Core.Interfaces;
using DropSort.Core.Models;

namespace Infrastructure.FileSystem;

public class FileMover : IFileMover
{
    private readonly IRenameService _rename;
    private readonly IDuplicateResolver _duplicate;

    public FileMover(
        IRenameService rename,
        IDuplicateResolver duplicate)
    {
        _rename = rename;
        _duplicate = duplicate;
    }

    public async Task MoveAsync(FileItem file, string targetPath, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var targetDir = Path.GetDirectoryName(targetPath)!;
        Directory.CreateDirectory(targetDir);

        var normalizedName = _rename.Normalize(Path.GetFileName(targetPath));
        var renamedPath = Path.Combine(targetDir, normalizedName);

        var finalPath = _duplicate.Resolve(renamedPath);

        const int maxRetry = 3;
        for (int i = 0; i < maxRetry; i++)
        {
            try
            {
                File.Move(file.FullPath, finalPath);
                file.FullPath = finalPath;
                return;
            }
            catch (IOException)
            {
                if (i == maxRetry - 1)
                    throw;

                await Task.Delay(200, ct);
            }
        }
    }

}