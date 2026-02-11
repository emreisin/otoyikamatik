using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartController.Db;

namespace SmartController.Web.Controllers;

public class CommandsController : BaseController
{
    private readonly SmartControllerDbContext _db;

    public CommandsController(SmartControllerDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var query = _db.Commands
            .Include(c => c.Device)
            .ThenInclude(d => d.Sube)
            .AsQueryable();

        // Distribütör filtresi
        if (!IsAdmin && DistributorId.HasValue)
            query = query.Where(c => c.Device.Sube.DistributorId == DistributorId);

        var commands = await query
            .OrderByDescending(c => c.CreatedAt)
            .Take(100)
            .ToListAsync();

        return View(commands);
    }
}
