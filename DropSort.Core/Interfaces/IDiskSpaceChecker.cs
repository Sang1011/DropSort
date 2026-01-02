namespace DropSort.Core.Interfaces;

public interface IDiskSpaceChecker
{
    bool IsDiskFull(string targetPath);
}