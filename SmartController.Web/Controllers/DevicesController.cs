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

        return View(device);
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
