using Microsoft.EntityFrameworkCore;
using SmartController.Db;
using SmartController.Db.Entities;
using SmartController.Shared.Enums;

namespace SmartController.Api.Services;

public class ScheduledJobService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScheduledJobService> _logger;

    public ScheduledJobService(IServiceProvider serviceProvider, ILogger<ScheduledJobService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ScheduledJobService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessScheduledJobs(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scheduled jobs");
            }

            // Her dakika kontrol et
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ProcessScheduledJobs(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SmartControllerDbContext>();

        var now = DateTime.UtcNow;
        var currentTime = TimeOnly.FromDateTime(now);
        var currentDayOfWeek = (int)now.DayOfWeek;
        var currentDayOfMonth = now.Day;

        var jobs = await db.ScheduledJobs
            .Include(j => j.Sube)
            .Where(j => j.IsActive)
            .ToListAsync(stoppingToken);

        foreach (var job in jobs)
        {
            if (ShouldExecute(job, now, currentTime, currentDayOfWeek, currentDayOfMonth))
            {
                _logger.LogInformation("Executing scheduled job {JobId} for Sube {SubeId}", job.Id, job.SubeId);
                await ExecuteJob(db, job, stoppingToken);
            }
        }
    }

    private bool ShouldExecute(ScheduledJob job, DateTime now, TimeOnly currentTime, int currentDayOfWeek, int currentDayOfMonth)
    {
        // Zaman kontrolü (±1 dakika tolerans)
        var timeDiff = Math.Abs((currentTime.ToTimeSpan() - job.ExecutionTime.ToTimeSpan()).TotalMinutes);
        if (timeDiff > 1) return false;

        // Bugün zaten çalıştı mı?
        if (job.LastExecutedAt.HasValue && job.LastExecutedAt.Value.Date == now.Date)
            return false;

        // Frekans kontrolü
        return job.Frequency switch
        {
            ScheduleFrequency.Daily => true,
            ScheduleFrequency.Weekly => job.DayOfWeek == currentDayOfWeek,
            ScheduleFrequency.Monthly => job.DayOfMonth == currentDayOfMonth,
            _ => false
        };
    }

    private async Task ExecuteJob(SmartControllerDbContext db, ScheduledJob job, CancellationToken stoppingToken)
    {
        // Şubedeki tüm cihazları al
        var devices = await db.Devices
            .Include(d => d.Counters)
            .Where(d => d.SubeId == job.SubeId)
            .ToListAsync(stoppingToken);

        if (!devices.Any())
        {
            _logger.LogWarning("No devices found for Sube {SubeId}", job.SubeId);
            job.LastExecutedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(stoppingToken);
            return;
        }

        // Sistem kullanıcısı olarak rapor oluştur (UserId = 1 admin)
        var adminUser = await db.Users.FirstOrDefaultAsync(u => u.Role == UserRole.Admin, stoppingToken);
        var userId = adminUser?.Id ?? 1;

        // Z Raporu oluştur
        var report = new Report
        {
            SubeId = job.SubeId,
            ReportType = ReportType.Z,
            CreatedByUserId = userId,
            IsAutomatic = true
        };

        foreach (var device in devices)
        {
            foreach (var counter in device.Counters)
            {
                report.Details.Add(new ReportDetail
                {
                    DeviceId = device.DeviceId,
                    CounterType = counter.CounterType,
                    CountValue = counter.TotalCount
                });
            }

            // CT komutu gönder
            db.Commands.Add(new Command
            {
                DeviceId = device.DeviceId,
                CommandText = "{CT}",
                Status = CommandStatus.Pending
            });
        }

        db.Reports.Add(report);
        job.LastExecutedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(stoppingToken);

        _logger.LogInformation("Scheduled job {JobId} completed. Report {ReportId} created with {DeviceCount} devices",
            job.Id, report.Id, devices.Count);
    }
}
