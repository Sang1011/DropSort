using Application;
using Infrastructure;
using Infrastructure.Persistence.Sqlite;
using DropSort.Core.Interfaces;
using DropSort.Worker;
using Infrastructure.Persistence.Repositories;

var builder = Host.CreateApplicationBuilder(args);

// ===== DB PATH =====
var dbPath = Path.Combine(AppContext.BaseDirectory, "dropsort.db");

// ===== BOOTSTRAP DB + CONFIG =====
{
    var services = new ServiceCollection();
    services.AddSingleton(new SqliteConnectionFactory(dbPath));
    services.AddSingleton<DatabaseInitializer>();
    services.AddSingleton<ISettingRepository, SettingRepository>();

    var provider = services.BuildServiceProvider();

    // 1. Init tables
    provider.GetRequiredService<DatabaseInitializer>().Initialize();

    // 2. Seed REQUIRED config
    var settings = provider.GetRequiredService<ISettingRepository>();
    if (string.IsNullOrWhiteSpace(settings.Get("download_root")))
    {
        settings.Set("download_root", @"D:\SortedDownloads");
        Console.WriteLine("Seeded download_root = D:\\SortedDownloads");
    }
}

// ===== DI =====
builder.Services.AddApplication();
builder.Services.AddInfrastructure(dbPath);

// ===== WORKER =====
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<QueueHostedService>();

var host = builder.Build();
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    try
    {
        using var scope = host.Services.CreateScope();
        var logRepo = scope.ServiceProvider.GetRequiredService<ILogRepository>();

        var logs = logRepo.GetLatestAsync(500).GetAwaiter().GetResult();

        var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDir);

        var filePath = Path.Combine(
            logDir,
            $"dropsort-last-session-{DateTime.UtcNow:yyyyMMdd-HHmmss}.txt"
        );

        File.WriteAllLines(
            filePath,
            logs.Select(l =>
                $"[{l.CreatedAt:yyyy-MM-dd HH:mm:ss}] [{l.Level}] {l.Message} {l.FileName}"
            )
        );
    }
    catch
    {
    }
});

host.Run();