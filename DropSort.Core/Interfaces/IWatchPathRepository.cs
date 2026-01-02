namespace DropSort.Core.Interfaces;

public interface IWatchPathRepository
{
    IReadOnlyList<string> GetEnabledPaths();

    Task AddAsync(string path, bool enabled = true);

    Task DeleteAsync(string path);

    Task SetEnabledAsync(string path, bool enabled);
}