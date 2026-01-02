using DropSort.Core.Interfaces;
using Infrastructure.FileSystem;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Sqlite;
using Infrastructure.System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string dbPath)
    {
        // ===== SQLITE =====
        services.AddSingleton(new SqliteConnectionFactory(dbPath));
        services.AddSingleton<DatabaseInitializer>();

        // ===== REPOSITORIES =====
        services.AddSingleton<ISettingRepository, SettingRepository>();
        services.AddSingleton<ILogRepository, LogRepository>();
        services.AddSingleton<IWatchPathRepository, WatchPathRepository>();
        services.AddSingleton<IKeywordRuleRepository, KeywordRuleRepository>();
        services.AddSingleton<IFileTaskRepository, FileTaskRepository>();

        // ===== FILE SYSTEM =====
        services.AddSingleton<IFileMover, FileMover>();
        services.AddSingleton<IDuplicateResolver, DuplicateResolver>();
        services.AddSingleton<ISystemIdleChecker, SystemIdleChecker>();
        services.AddSingleton<IDiskSpaceChecker, DiskSpaceChecker>();

        // ===== FILE WATCHER =====
        services.AddSingleton<IFileWatcher>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var settings = sp.GetRequiredService<ISettingRepository>();
            var watchRepo = sp.GetRequiredService<IWatchPathRepository>();

            // ===== TARGET ROOT =====
            var targetRoot = settings.Get("download_root");
            if (string.IsNullOrWhiteSpace(targetRoot))
                throw new InvalidOperationException("download_root is required");

            Directory.CreateDirectory(targetRoot);

            // ===== WATCH PATHS =====
            var watchPaths = watchRepo.GetEnabledPaths();

            // ===== FALLBACK =====
            if (watchPaths.Count == 0)
            {
                var fallback = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads"
                );

                watchPaths = new[] { fallback };

                var logger = loggerFactory.CreateLogger("WatchPathFallback");
                logger.LogWarning(
                    "No enabled watch paths found. Fallback to default Downloads: {path}",
                    fallback
                );
            }

            var distinctPaths = watchPaths
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var watchers = new List<IFileWatcher>();

            foreach (var path in distinctPaths)
            {
                Directory.CreateDirectory(path);

                if (string.Equals(
                        Path.GetFullPath(path),
                        Path.GetFullPath(targetRoot),
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"Watch path MUST NOT equal download_root: {path}");
                }

                var watcherLogger = loggerFactory.CreateLogger<FileWatcherService>();

                watchers.Add(new FileWatcherService(
                    watcherLogger,
                    path,
                    targetRoot
                ));
            }

            // ===== 🔥 OPTIMIZATION POINT =====
            if (watchers.Count == 1)
            {
                return watchers[0];
            }

            return new MultiFileWatcherService(watchers);
        });

        return services;
    }
}