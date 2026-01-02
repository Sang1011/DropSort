using DropSort.Core.Interfaces;

namespace Infrastructure.System;

public class DiskSpaceChecker : IDiskSpaceChecker
{
    private const long MinFreeBytes = 500L * 1024 * 1024;

    public bool IsDiskFull(string targetPath)
    {
        var root = Path.GetPathRoot(targetPath);
        if (string.IsNullOrWhiteSpace(root))
            return true;

        var drive = new DriveInfo(root);
        return drive.AvailableFreeSpace < MinFreeBytes;
    }
}