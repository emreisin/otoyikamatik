using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartController.Db;
using SmartController.Db.Entities;
using SmartController.Shared.Enums;

namespace SmartController.Web.Controllers;

public class ReportsController : BaseController
{
    private readonly SmartControllerDbContext _db;

    public ReportsController(SmartControllerDbContext db) => _db = db;

    public async Task<IActionResult> Index(int? subeId)
    {
        var query = _db.Reports
            .Include(r => r.Sube)
            .Include(r => r.CreatedBy)
            .Include(r => r.Details)
            .AsQueryable();

        if (!IsAdmin && DistributorId.HasValue)
            query = query.Where(r => r.Sube.DistributorId == DistributorId);

        if (subeId.HasValue)
            query = query.Where(r => r.SubeId == subeId);

        var subelerQuery = _db.Subeler.AsQueryable();
        if (!IsAdmin && DistributorId.HasValue)
            subelerQuery = subelerQuery.Where(s => s.DistributorId == DistributorId);

        ViewBag.Subeler = await subelerQuery.ToListAsync();
        ViewBag.SelectedSubeId = subeId;

        return View(await query.OrderByDescending(r => r.ReportDate).Take(100).ToListAsync());
    }

    public async Task<IActionResult> Details(long id)
    {
        var report = await _db.Reports
            .Include(r => r.Sube)
            .Include(r => r.CreatedBy)
            .Include(r => r.Details)
            .ThenInclude(d => d.Device)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (report == null) return NotFound();

        if (!IsAdmin && DistributorId.HasValue && report.Sube.DistributorId != DistributorId)
            return Forbid();

        return View(report);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var subelerQuery = _db.Subeler.AsQueryable();
        if (!IsAdmin && DistributorId.HasValue)
            subelerQuery = subelerQuery.Where(s => s.DistributorId == DistributorId);

        ViewBag.Subeler = await subelerQuery.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int subeId, ReportType reportType)
    {
        var sube = await _db.Subeler.FindAsync(subeId);
        if (sube == null) return NotFound();

        if (!IsAdmin && DistributorId.HasValue && sube.DistributorId != DistributorId)
            return Forbid();

        // Şubedeki tüm cihazların sayaçlarını al
        var devices = await _db.Devices
            .Include(d => d.Counters)
            .Where(d => d.SubeId == subeId)
            .ToListAsync();

        // Rapor oluştur
        var report = new Report
        {
            SubeId = subeId,
            ReportType = reportType,
            CreatedByUserId = UserId,
            IsAutomatic = false
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
        }

        _db.Reports.Add(report);

        // Z Raporu ise sayaçları sıfırla ve CT komutu gönder
        if (reportType == ReportType.Z)
        {
            foreach (var device in devices)
            {
                // CT komutu ekle
                _db.Commands.Add(new Command
                {
                    DeviceId = device.DeviceId,
                    CommandText = "{CT}",
                    Status = CommandStatus.Pending
                });
            }
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = reportType == ReportType.X 
            ? "X Raporu oluşturuldu" 
            : "Z Raporu oluşturuldu ve sayaç sıfırlama komutları gönderildi";

        return RedirectToAction(nameof(Details), new { id = report.Id });
    }
}
