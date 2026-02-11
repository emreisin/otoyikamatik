using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartController.Db;
using SmartController.Shared.DTOs;

namespace SmartController.Web.Controllers;

public class HomeController : BaseController
{
    private readonly SmartControllerDbContext _db;

    public HomeController(SmartControllerDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var last24h = now.AddHours(-24);

        var devicesQuery = _db.Devices.AsQueryable();
        var rawDataQuery = _db.DeviceRawData.AsQueryable();

        // Distribütör filtresi
        if (!IsAdmin && DistributorId.HasValue)
        {
            var subeIds = await _db.Subeler
                .Where(s => s.DistributorId == DistributorId)
                .Select(s => s.Id)
                .ToListAsync();

            devicesQuery = devicesQuery.Where(d => subeIds.Contains(d.SubeId));
            rawDataQuery = rawDataQuery.Where(r => subeIds.Contains(r.Device.SubeId));
        }

        var dashboard = new DashboardDto
        {
            TotalDevices = await devicesQuery.CountAsync(),
            OnlineDevices = await devicesQuery.CountAsync(d => d.IsOnline),
            OfflineDevices = await devicesQuery.CountAsync(d => !d.IsOnline),
            TodayOperations = await rawDataQuery.CountAsync(r => r.CreatedAt >= today),
            Last24HoursAlarms = await devicesQuery.CountAsync(d => d.LastSeen < last24h)
        };

        return View(dashboard);
    }
}
