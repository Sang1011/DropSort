namespace DropSort.Core.Interfaces;

public interface IWatchPathRepository
{
    IReadOnlyList<string> GetEnabledPaths();
}