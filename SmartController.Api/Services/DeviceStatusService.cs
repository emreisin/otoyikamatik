using Microsoft.EntityFrameworkCore;
using SmartController.Db;

namespace SmartController.Api.Services;

public class DeviceStatusService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DeviceStatusService> _logger;

    public DeviceStatusService(IServiceProvider serviceProvider, ILogger<DeviceStatusService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DeviceStatusService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateOfflineDevices(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device status");
            }

            // Her 2 dakikada kontrol et
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        }
    }

    private async Task UpdateOfflineDevices(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SmartControllerDbContext>();

        var threshold = DateTime.UtcNow.AddMinutes(-5);

        var offlineDevices = await db.Devices
            .Where(d => d.IsOnline && d.LastSeen < threshold)
            .ToListAsync(stoppingToken);

        if (offlineDevices.Any())
        {
            foreach (var device in offlineDevices)
            {
                device.IsOnline = false;
            }

            await db.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("Marked {Count} devices as offline", offlineDevices.Count);
        }
    }
}
