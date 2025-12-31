using DropSort.Core.Interfaces;
using DropSort.Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Infrastructure.FileSystem;

public class FileWatcherService : IFileWatcher
{
    private readonly ILogger<FileWatcherService> _logger;
    private readonly FileSystemWatcher _watcher;
    private readonly string _targetRoot;
    private CancellationTokenSource? _cts;

    // debounce: path → last write time
    private readonly ConcurrentDictionary<string, DateTime> _pendingFiles = new();

    public event Action<FileItem>? FileReady;

    public FileWatcherService(
        ILogger<FileWatcherService> logger,
        string watchPath,
        string targetRoot)
    {
        _logger = logger;
        _targetRoot = Path.GetFullPath(targetRoot);
        _watcher = new FileSystemWatcher(watchPath)
        {
            IncludeSubdirectories = false,
            EnableRaisingEvents = false,
            NotifyFilter =
                NotifyFilters.FileName |
                NotifyFilters.Size |
                NotifyFilters.LastWrite
        };

        _watcher.Created += OnFileChanged;
        _watcher.Changed += OnFileChanged;
    }

    public void Start()
    {
        _logger.LogInformation("FileWatcher started");
        _watcher.EnableRaisingEvents = true;

        _cts = new CancellationTokenSource();
        _ = Task.Run(() => ProcessPendingFilesAsync(_cts.Token));
    }

    public void Stop()
    {
        _logger.LogInformation("FileWatcher stopped");
        _watcher.EnableRaisingEvents = false;
        _cts?.Cancel();
    }
    
    private async Task ProcessPendingFilesAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            foreach (var kv in _pendingFiles.ToArray())
            {
                var path = kv.Key;
                var lastEvent = kv.Value;

                if ((DateTime.UtcNow - lastEvent).TotalMilliseconds < 1200)
                    continue;

                if (!File.Exists(path))
                {
                    _pendingFiles.TryRemove(path, out _);
                    continue;
                }

                if (!IsFileStable(path))
                    continue;

                if (_pendingFiles.TryRemove(path, out _))
                {
                    var info = new FileInfo(path);

                    var item = new FileItem
                    {
                        FullPath = path,
                        FileName = info.Name,
                        Extension = info.Extension,
                        SizeInBytes = info.Length,
                        SourceDrive = Path.GetPathRoot(path) ?? ""
                    };

                    _logger.LogInformation("File ready: {file}", info.Name);
                    FileReady?.Invoke(item);
                }
            }

            await Task.Delay(300, ct);
        }
    }



    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (Directory.Exists(e.FullPath))
            return;

        _logger.LogDebug(
            "FS EVENT: {type} - {path}",
            e.ChangeType,
            e.FullPath
        );

        var fullPath = Path.GetFullPath(e.FullPath);

        if (fullPath.StartsWith(_targetRoot, StringComparison.OrdinalIgnoreCase))
            return;

        if (IsIgnored(fullPath))
            return;

        if (!File.Exists(fullPath))
            return;

        _pendingFiles[fullPath] = DateTime.UtcNow;
    }


    private bool IsFileStable(string path)
    {
        try
        {
            using var stream = File.Open(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.None
            );
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsIgnored(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext is ".tmp" or ".crdownload" or ".part";
    }
}
