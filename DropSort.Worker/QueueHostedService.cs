using Application.Services;
namespace DropSort.Worker;

public class QueueHostedService : BackgroundService
{
    private readonly QueueProcessingService _processor;
    private readonly ILogger<QueueHostedService> _logger;

    public QueueHostedService(
        QueueProcessingService processor,
        ILogger<QueueHostedService> logger)
    {
        _processor = processor;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("QueueHostedService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            await _processor.ProcessOnceAsync(stoppingToken);
            await Task.Delay(500, stoppingToken);
        }
    }
}