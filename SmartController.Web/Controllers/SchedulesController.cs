using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartController.Db;
using SmartController.Db.Entities;
using SmartController.Shared.Enums;

namespace SmartController.Web.Controllers;

public class SchedulesController : BaseController
{
    private readonly SmartControllerDbContext _db;

    public SchedulesController(SmartControllerDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var query = _db.ScheduledJobs.Include(j => j.Sube).ThenInclude(s => s.Distributor).AsQueryable();

        if (!IsAdmin && DistributorId.HasValue)
            query = query.Where(j => j.Sube.DistributorId == DistributorId);

        return View(await query.ToListAsync());
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
    public async Task<IActionResult> Create(int subeId, ScheduleFrequency frequency, string executionTime, int? dayOfWeek, int? dayOfMonth)
    {
        var sube = await _db.Subeler.FindAsync(subeId);
        if (sube == null) return NotFound();

        if (!IsAdmin && DistributorId.HasValue && sube.DistributorId != DistributorId)
            return Forbid();

        var job = new ScheduledJob
        {
            SubeId = subeId,
            Frequency = frequency,
            ExecutionTime = TimeOnly.Parse(executionTime),
            DayOfWeek = frequency == ScheduleFrequency.Weekly ? dayOfWeek : null,
            DayOfMonth = frequency == ScheduleFrequency.Monthly ? dayOfMonth : null,
            IsActive = true
        };

        _db.ScheduledJobs.Add(job);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Zamanlanmış görev oluşturuldu";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Toggle(int id)
    {
        var job = await _db.ScheduledJobs.Include(j => j.Sube).FirstOrDefaultAsync(j => j.Id == id);
        if (job == null) return NotFound();

        if (!IsAdmin && DistributorId.HasValue && job.Sube.DistributorId != DistributorId)
            return Forbid();

        job.IsActive = !job.IsActive;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var job = await _db.ScheduledJobs.Include(j => j.Sube).FirstOrDefaultAsync(j => j.Id == id);
        if (job == null) return NotFound();

        if (!IsAdmin && DistributorId.HasValue && job.Sube.DistributorId != DistributorId)
            return Forbid();

        _db.ScheduledJobs.Remove(job);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
