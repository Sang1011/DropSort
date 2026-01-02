using Application.Queues;
using Application.Services;
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

    public Worker(
        ILogger<Worker> logger,
        DatabaseInitializer dbInitializer,
        ISettingRepository settingRepo,
        IFileWatcher fileWatcher,
        QueueDecisionService decisionService,
        IFileTaskRepository taskRepo,
        InMemoryQueues queues,
        QueueProcessingService queueProcessor)
    {
        _logger = logger;
        _dbInitializer = dbInitializer;
        _settingRepo = settingRepo;
        _fileWatcher = fileWatcher;
        _decisionService = decisionService;
        _taskRepo = taskRepo;
        _queues = queues;
        _queueProcessor = queueProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DropSort Worker starting...");

        // ===== INIT DB =====
        _dbInitializer.Initialize();
        _logger.LogInformation("Database initialized");

        // ===== RESUME PENDING TASKS =====
        _logger.LogInformation("Resuming pending tasks...");

        var pendingTasks = await _taskRepo.GetPendingAsync();
        foreach (var task in pendingTasks)
            _queues.High.Enqueue(task);

        _logger.LogInformation("Resumed {count} pending tasks", pendingTasks.Count);

        var root = _settingRepo.Get("download_root");
        if (string.IsNullOrWhiteSpace(root))
            throw new InvalidOperationException("download_root is not configured.");

        // ===== FILE WATCHER =====
        _fileWatcher.FileReady += OnFileReady;
        _fileWatcher.Start();

        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Stopping FileWatcher...");
            _fileWatcher.Stop();
        });

        // ===== QUEUE PROCESS LOOP (CÁI BẠN THIẾU) =====
        _ = Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _queueProcessor.ProcessOnceAsync(stoppingToken);
                await Task.Delay(200, stoppingToken);
            }
        }, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }


    private void OnFileReady(FileItem file)
    {
        _logger.LogInformation(
            "Detected downloaded file: {file} ({size} bytes)",
            file.FileName,
            file.SizeInBytes
        );

        _logger.LogInformation(
            "File ready → decision: {file}",
            file.FileName
        );

        _decisionService.DecideAsync(file);
    }
}
