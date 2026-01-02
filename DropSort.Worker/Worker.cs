using Application.Queues;
using Application.Services;
using DropSort.Core.Enums;
using DropSort.Core.Interfaces;
using DropSort.Core.Models;
using Infrastructure.Persistence.Sqlite;

namespace DropSort.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly DatabaseInitializer _dbInitializer;
    private readonly ISettingRepository _settingRepo;
    private readonly IFileWatcher _fileWatcher;
    private readonly QueueDecisionService _decisionService;
    private readonly IFileTaskRepository _taskRepo;
    private readonly InMemoryQueues _queues;
    private readonly QueueProcessingService _queueProcessor;
    private readonly ILogService _log;

    public Worker(
        ILogger<Worker> logger,
        DatabaseInitializer dbInitializer,
        ISettingRepository settingRepo,
        IFileWatcher fileWatcher,
        QueueDecisionService decisionService,
        IFileTaskRepository taskRepo,
        InMemoryQueues queues,
        QueueProcessingService queueProcessor,
        ILogService log)
    {
        _logger = logger;
        _dbInitializer = dbInitializer;
        _settingRepo = settingRepo;
        _fileWatcher = fileWatcher;
        _decisionService = decisionService;
        _taskRepo = taskRepo;
        _queues = queues;
        _queueProcessor = queueProcessor;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DropSort Worker starting...");

        // ===== INIT DB =====
        _dbInitializer.Initialize();
        _logger.LogInformation("Database initialized");

        // ===== RESUME PENDING TASKS =====
        _logger.LogInformation("Resuming pending tasks...");

        try
        {
            var pendingTasks = await _taskRepo.GetPendingAsync();

            foreach (var task in pendingTasks)
                _queues.High.Enqueue(task);

            _logger.LogInformation(
                "Resumed {count} pending tasks",
                pendingTasks.Count
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to resume pending tasks. DropSort will continue without resuming."
            );

            await _log.ErrorAsync(
                LogEvent.Failed,
                ex,
                "resume_pending_tasks"
            );
        }

        // ===== FILE WATCHER =====
        _fileWatcher.FileReady += OnFileReady;
        _fileWatcher.Start();

        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Stopping FileWatcher...");
            _fileWatcher.Stop();
        });

        // ===== QUEUE PROCESS LOOP =====
        _ = Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _queueProcessor.ProcessOnceAsync(stoppingToken);
                await Task.Delay(200, stoppingToken);
            }
        }, stoppingToken);

        // ===== KEEP WORKER ALIVE =====
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }



    private async void OnFileReady(FileItem file)
    {
        try
        {
        await _log.InfoAsync(
            LogEvent.FileDetected,
            file.FileName,
            file.FullPath
        );

        _logger.LogDebug(
            "FileReady event: {file}, size={size}",
            file.FileName,
            file.SizeInBytes
        );

        await _decisionService.DecideAsync(file);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling FileReady for {file}", file.FileName);
        }
    }

}
