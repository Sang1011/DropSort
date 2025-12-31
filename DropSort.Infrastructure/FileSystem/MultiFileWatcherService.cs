using DropSort.Core.Interfaces;
using DropSort.Core.Models;

namespace Infrastructure.FileSystem;

public class MultiFileWatcherService : IFileWatcher
{
    private readonly IReadOnlyList<IFileWatcher> _watchers;

    public event Action<FileItem>? FileReady;

    public MultiFileWatcherService(IEnumerable<IFileWatcher> watchers)
    {
        _watchers = watchers.ToList();

        foreach (var watcher in _watchers)
        {
            watcher.FileReady += file => FileReady?.Invoke(file);
        }
    }

    public void Start()
    {
        foreach (var watcher in _watchers)
            watcher.Start();
    }

    public void Stop()
    {
        foreach (var watcher in _watchers)
            watcher.Stop();
    }
}