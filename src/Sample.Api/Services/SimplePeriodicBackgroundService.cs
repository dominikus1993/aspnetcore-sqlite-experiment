using Microsoft.EntityFrameworkCore;

using Sample.Api.Infrastructure.EntityFramework;

namespace Sample.Api.Services;

public sealed class SimplePeriodicBackgroundService : BackgroundService
{
    private readonly ILogger<SimplePeriodicBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public SimplePeriodicBackgroundService(ILogger<SimplePeriodicBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            await using var context = scope.ServiceProvider.GetRequiredService<SampleDbContext>();

            var recordsQuantity = await context.CountPersons(stoppingToken);
            
            _logger.LogInformation("Records quantity in database: {RecordsQuantity}", recordsQuantity);
        }
    }
}