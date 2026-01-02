using DropSort.Core.Enums;
using DropSort.Core.Interfaces;
using DropSort.Core.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class QueueDecisionService
{
    private const long LargeFileThresholdBytes = 200 * 1024 * 1024;

    private readonly IQueueProcessor _queue;
    private readonly ISettingRepository _settings;
    private readonly IFileTaskRepository _taskRepo;
    private readonly IFolderResolver _folderResolver;

    public QueueDecisionService(
        IQueueProcessor queue,
        ISettingRepository settings,
        IFileTaskRepository taskRepo,
        IFolderResolver folderResolver)
    {
        _queue = queue;
        _settings = settings;
        _taskRepo = taskRepo;
        _folderResolver = folderResolver;
    }

    public async Task DecideAsync(FileItem file)
    {
        var existing = await _taskRepo.GetByPathAsync(file.FullPath);
        if (existing?.Status == FileTaskStatus.Moved)
            return;

        var targetRoot = _settings.Get("download_root");
        if (string.IsNullOrWhiteSpace(targetRoot))
            return;

        var targetDrive = Path.GetPathRoot(targetRoot);
        var sameDrive = string.Equals(
            file.SourceDrive,
            targetDrive,
            StringComparison.OrdinalIgnoreCase);
        var targetPath = _folderResolver.ResolveTargetPath(file);
        var task = new FileTask
        {
            Id = Guid.NewGuid().ToString(),
            FullPath = file.FullPath,
            FileName = file.FileName,
            Extension = file.Extension,
            SizeInBytes = file.SizeInBytes,
            SourceDrive = file.SourceDrive,
            Category = file.Category,
            TargetPath = targetPath,
            CreatedAt = DateTime.UtcNow,
            Status = FileTaskStatus.Pending
        };

        await _taskRepo.AddAsync(task);

        if (sameDrive || file.SizeInBytes < LargeFileThresholdBytes)
            _queue.EnqueueHigh(task);
        else
            _queue.EnqueueLow(task);
    }
}
