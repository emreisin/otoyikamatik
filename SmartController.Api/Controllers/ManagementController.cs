using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartController.Db;
using SmartController.Db.Entities;
using SmartController.Shared.DTOs;
using SmartController.Shared.Enums;

namespace SmartController.Api.Controllers;

[ApiController]
[Route("api")]
public class ManagementController : ControllerBase
{
    private readonly SmartControllerDbContext _db;

    public ManagementController(SmartControllerDbContext db) => _db = db;

    [HttpGet("devices")]
    public async Task<IActionResult> GetDevices([FromQuery] int? subeId, [FromQuery] int? peronId)
    {
        var query = _db.Devices.Include(d => d.Sube).Include(d => d.Peron).AsQueryable();
        
        if (subeId.HasValue) query = query.Where(d => d.SubeId == subeId);
        if (peronId.HasValue) query = query.Where(d => d.PeronId == peronId);

        return Ok(await query.ToListAsync());
    }

    [HttpGet("devices/{id}")]
    public async Task<IActionResult> GetDevice(string id)
    {
        var device = await _db.Devices
            .Include(d => d.Sube)
            .Include(d => d.Peron)
            .FirstOrDefaultAsync(d => d.DeviceId == id);

        return device == null ? NotFound() : Ok(device);
    }

    [HttpGet("devices/{id}/counters")]
    public async Task<IActionResult> GetCounters(string id)
    {
        var counters = await _db.DeviceCounters.Where(c => c.DeviceId == id).ToListAsync();
        return Ok(counters);
    }

    [HttpGet("devices/{id}/logs")]
    public async Task<IActionResult> GetLogs(string id, [FromQuery] int take = 50)
    {
        var logs = await _db.DeviceStatusLogs
            .Where(l => l.DeviceId == id)
            .OrderByDescending(l => l.CreatedAt)
            .Take(take)
            .ToListAsync();
        return Ok(logs);
    }

    [HttpPost("commands/send")]
    public async Task<IActionResult> SendCommand([FromBody] SendCommandDto dto)
    {
        var device = await _db.Devices.FindAsync(dto.DeviceId);
        if (device == null) return NotFound();

        var command = new Command
        {
            DeviceId = dto.DeviceId,
            CommandText = dto.CommandText,
            Status = CommandStatus.Pending
        };
        _db.Commands.Add(command);
        await _db.SaveChangesAsync();

        return Ok(new { commandId = command.Id });
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var last24h = now.AddHours(-24);

        var dto = new DashboardDto
        {
            TotalDevices = await _db.Devices.CountAsync(),
            OnlineDevices = await _db.Devices.CountAsync(d => d.IsOnline),
            OfflineDevices = await _db.Devices.CountAsync(d => !d.IsOnline),
            TodayOperations = await _db.DeviceRawData.CountAsync(r => r.CreatedAt >= today),
            Last24HoursAlarms = await _db.Devices.CountAsync(d => d.LastSeen < last24h)
        };
        return Ok(dto);
    }
}
