using DropSort.Core.Interfaces;
using DropSort.Core.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class QueueDecisionService
{
    private const long LargeFileThresholdBytes = 200 * 1024 * 1024; 

    private readonly IQueueProcessor _queue;
    private readonly ISettingRepository _settings;
    private readonly ILogger<QueueDecisionService> _logger;

    public QueueDecisionService(
        IQueueProcessor queue,
        ISettingRepository settings,
        ILogger<QueueDecisionService> logger)
    {
        _queue = queue;
        _settings = settings;
        _logger = logger;
    }

    public void Decide(FileItem file)
    {
        var targetRoot = _settings.Get("download_root");
        if (string.IsNullOrWhiteSpace(targetRoot))
        {
            _logger.LogWarning(
                "No download_root configured, skipping {file}",
                file.FileName);
            return;
        }

        var targetDrive = Path.GetPathRoot(targetRoot);

        var sameDrive =
            string.Equals(file.SourceDrive, targetDrive,
                StringComparison.OrdinalIgnoreCase);

        if (sameDrive)
        {
            _queue.EnqueueHigh(file);
            return;
        }

        // khác ổ
        if (file.SizeInBytes >= LargeFileThresholdBytes)
        {
            _queue.EnqueueLow(file);
            // bước sau sẽ notify
        }
        else
        {
            _queue.EnqueueHigh(file);
        }
    }
}