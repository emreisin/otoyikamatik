using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartController.Db;
using SmartController.Db.Entities;
using SmartController.Shared.DTOs;

namespace SmartController.Api.Controllers;

[ApiController]
[Route("device")]
public class DeviceController : ControllerBase
{
    private readonly SmartControllerDbContext _db;

    public DeviceController(SmartControllerDbContext db) => _db = db;

    [HttpPost("status/{customerSuffix}")]
    public async Task<IActionResult> PostStatus(string customerSuffix, [FromBody] DeviceStatusDto dto)
    {
        var device = await _db.Devices.FirstOrDefaultAsync(d => d.DeviceId == dto.DeviceId);
        if (device == null)
        {
            var sube = await _db.Subeler.FirstOrDefaultAsync(s => s.Kod == dto.Sube);
            if (sube == null)
            {
                sube = new Sube { Kod = dto.Sube, Ad = dto.Sube };
                _db.Subeler.Add(sube);
                await _db.SaveChangesAsync();
            }

            var peron = await _db.Peronlar.FirstOrDefaultAsync(p => p.SubeId == sube.Id && p.PeronNo == dto.PeronId);
            if (peron == null)
            {
                peron = new Peron { SubeId = sube.Id, PeronNo = dto.PeronId };
                _db.Peronlar.Add(peron);
                await _db.SaveChangesAsync();
            }

            device = new Device
            {
                DeviceId = dto.DeviceId,
                DeviceName = dto.DeviceName,
                SubeId = sube.Id,
                PeronId = peron.Id
            };
            _db.Devices.Add(device);
        }

        device.FirmwareVersion = dto.FirmwareVersion;
        device.LastSeen = DateTime.UtcNow;
        device.IsOnline = true;

        _db.DeviceStatusLogs.Add(new DeviceStatusLog
        {
            DeviceId = dto.DeviceId,
            WifiSsid = dto.WifiSsid,
            WifiSignal = dto.WifiSignal,
            IpAddress = dto.IpAddress,
            Uptime = dto.Uptime
        });

        await _db.SaveChangesAsync();
        return Ok(new { success = true });
    }
}
