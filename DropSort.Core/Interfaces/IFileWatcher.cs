using DropSort.Core.Models;

namespace DropSort.Core.Interfaces;

public interface IFileWatcher
{
    void Start();
    void Stop();

    event Action<FileItem> FileReady;
}