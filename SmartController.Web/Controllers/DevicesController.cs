using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartController.Db;
using SmartController.Db.Entities;
using SmartController.Shared.Enums;

namespace SmartController.Web.Controllers;

public class DevicesController : BaseController
{
    private readonly SmartControllerDbContext _db;

    public DevicesController(SmartControllerDbContext db) => _db = db;

    public async Task<IActionResult> Index(int? subeId, int? peronId)
    {
        var query = _db.Devices.Include(d => d.Sube).ThenInclude(s => s.Distributor).Include(d => d.Peron).AsQueryable();

        // Distribütör filtresi
        if (!IsAdmin && DistributorId.HasValue)
            query = query.Where(d => d.Sube.DistributorId == DistributorId);

        if (subeId.HasValue) query = query.Where(d => d.SubeId == subeId);
        if (peronId.HasValue) query = query.Where(d => d.PeronId == peronId);

        var subelerQuery = _db.Subeler.AsQueryable();
        if (!IsAdmin && DistributorId.HasValue)
            subelerQuery = subelerQuery.Where(s => s.DistributorId == DistributorId);

        ViewBag.Subeler = await subelerQuery.ToListAsync();
        ViewBag.IsAdmin = IsAdmin;
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(string id)
    {
        var query = _db.Devices
            .Include(d => d.Sube)
            .Include(d => d.Peron)
            .Include(d => d.StatusLogs.OrderByDescending(l => l.CreatedAt).Take(10))
            .Include(d => d.Counters)
            .AsQueryable();

        // Distribütör filtresi
        if (!IsAdmin && DistributorId.HasValue)
            query = query.Where(d => d.Sube.DistributorId == DistributorId);

        var device = await query.FirstOrDefaultAsync(d => d.DeviceId == id);
        if (device == null) return NotFound();

        ViewBag.IsAdmin = IsAdmin;
        return View(device);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!IsAdmin) return Forbid();

        ViewBag.Subeler = await _db.Subeler.Include(s => s.Distributor).ToListAsync();
        ViewBag.Peronlar = await _db.Peronlar.Include(p => p.Sube).ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string deviceId, string deviceName, int subeId, int peronId, string? firmwareVersion)
    {
        if (!IsAdmin) return Forbid();

        // DeviceId kontrolü
        if (await _db.Devices.AnyAsync(d => d.DeviceId == deviceId))
        {
            TempData["Error"] = "Bu Device ID zaten mevcut";
            return RedirectToAction(nameof(Create));
        }

        var device = new Device
        {
            DeviceId = deviceId,
            DeviceName = deviceName,
            SubeId = subeId,
            PeronId = peronId,
            FirmwareVersion = firmwareVersion ?? "",
            IsOnline = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.Devices.Add(device);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Cihaz başarıyla eklendi";
        return RedirectToAction(nameof(Details), new { id = deviceId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        if (!IsAdmin) return Forbid();

        var device = await _db.Devices
            .Include(d => d.StatusLogs)
            .Include(d => d.RawDataLogs)
            .Include(d => d.Counters)
            .Include(d => d.Commands)
            .FirstOrDefaultAsync(d => d.DeviceId == id);

        if (device == null) return NotFound();

        // İlişkili verileri sil
        _db.DeviceStatusLogs.RemoveRange(device.StatusLogs);
        _db.DeviceRawData.RemoveRange(device.RawDataLogs);
        _db.DeviceCounters.RemoveRange(device.Counters);
        _db.Commands.RemoveRange(device.Commands);
        _db.Devices.Remove(device);

        await _db.SaveChangesAsync();

        TempData["Success"] = "Cihaz başarıyla silindi";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SendCommand(string deviceId, string commandText)
    {
        var device = await _db.Devices.Include(d => d.Sube).FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        if (device == null) return NotFound();

        // Yetki kontrolü
        if (!IsAdmin && DistributorId.HasValue && device.Sube.DistributorId != DistributorId)
            return Forbid();

        _db.Commands.Add(new Command
        {
            DeviceId = deviceId,
            CommandText = commandText,
            Status = CommandStatus.Pending
        });
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = deviceId });
    }
}
