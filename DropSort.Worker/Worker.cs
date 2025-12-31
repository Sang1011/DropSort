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

    public Worker(
        ILogger<Worker> logger,
        DatabaseInitializer dbInitializer,
        ISettingRepository settingRepo,
        IFileWatcher fileWatcher,
        QueueDecisionService decisionService)
    {
        _logger = logger;
        _dbInitializer = dbInitializer;
        _settingRepo = settingRepo;
        _fileWatcher = fileWatcher;
        _decisionService = decisionService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DropSort Worker starting...");

        // ===== INIT DB =====
        _dbInitializer.Initialize();
        _logger.LogInformation("Database initialized");
        
        var root = _settingRepo.Get("download_root");
        if (string.IsNullOrWhiteSpace(root))
        {
            throw new InvalidOperationException(
                "download_root is not configured. Please configure it before starting Worker.");
        }

        // ===== TEST SETTINGS =====
        _settingRepo.Set("worker_started_at", DateTime.UtcNow.ToString("O"));
        var value = _settingRepo.Get("worker_started_at");
        _logger.LogInformation("Setting test value = {value}", value);

        // ===== FILE WATCHER =====
        _fileWatcher.FileReady += OnFileReady;
        _fileWatcher.Start();

        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Stopping FileWatcher...");
            _fileWatcher.Stop();
        });

        // ===== GIỮ WORKER SỐNG =====
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

        _decisionService.Decide(file);
    }
}
