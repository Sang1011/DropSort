using Microsoft.Extensions.DependencyInjection;
using DropSort.Core.Interfaces;
using Application.Services;
using Application.Queues;
using Application.Services.Logging;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IFileClassifier, FileClassifier>();
        services.AddSingleton<IRenameService, RenameService>();
        services.AddSingleton<InMemoryQueues>();
        services.AddSingleton<IQueueProcessor, QueueProcessor>();
        services.AddSingleton<QueueDecisionService>();
        services.AddSingleton<QueueProcessingService>();
        services.AddSingleton<IFileClassifier, FileClassifier>();
        services.AddSingleton<IFolderResolver, FolderResolver>();
        services.AddSingleton<ICategoryPathResolver, CategoryPathResolver>();
        services.AddSingleton<IKeywordRuleMatcher, KeywordRuleMatcher>();
        services.AddSingleton<ILogService, LogService>();
        return services;
    }
}