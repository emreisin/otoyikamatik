using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartController.Api.Services;
using SmartController.Db;
using SmartController.Db.Entities;
using SmartController.Shared.DTOs;

namespace SmartController.Api.Controllers;

[ApiController]
[Route("esp")]
public class EspController : ControllerBase
{
    private readonly SmartControllerDbContext _db;

    public EspController(SmartControllerDbContext db) => _db = db;

    [HttpPost("data/{customerSuffix}")]
    public async Task<IActionResult> PostData(string customerSuffix, [FromBody] DeviceDataDto dto)
    {
        // Distributor'ı bul
        var distributor = await _db.Distributors.FirstOrDefaultAsync(d => d.Kod == customerSuffix);
        if (distributor == null)
            return BadRequest(new { error = "Invalid customerSuffix" });

        var device = await _db.Devices.FirstOrDefaultAsync(d => d.DeviceId == dto.DeviceId);
        if (device == null)
        {
            var sube = await _db.Subeler.FirstOrDefaultAsync(s => s.Kod == dto.Sube && s.DistributorId == distributor.Id);
            if (sube == null)
            {
                sube = new Sube { DistributorId = distributor.Id, Kod = dto.Sube, Ad = dto.Sube };
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

        device.LastSeen = DateTime.UtcNow;
        device.IsOnline = true;

        // Raw data'yı kaydet
        var rawDataEntity = new DeviceRawData
        {
            DeviceId = dto.DeviceId,
            RawData = dto.RawData,
            Parsed = false
        };
        _db.DeviceRawData.Add(rawDataEntity);

        // Raw data'yı parse et ve sayaçları güncelle
        var parsedCounters = RawDataParser.Parse(dto.RawData);
        if (parsedCounters.Count > 0)
        {
            rawDataEntity.Parsed = true;

            foreach (var parsed in parsedCounters)
            {
                var counter = await _db.DeviceCounters
                    .FirstOrDefaultAsync(c => c.DeviceId == dto.DeviceId && c.CounterType == parsed.Channel);

                if (counter == null)
                {
                    counter = new DeviceCounter
                    {
                        DeviceId = dto.DeviceId,
                        CounterType = parsed.Channel,
                        TotalCount = parsed.Count,
                        LastIncrementAt = DateTime.UtcNow
                    };
                    _db.DeviceCounters.Add(counter);
                }
                else
                {
                    counter.TotalCount = parsed.Count;
                    counter.LastIncrementAt = DateTime.UtcNow;
                }
            }
        }

        await _db.SaveChangesAsync();
        return Ok(new { success = true, parsed = parsedCounters.Count });
    }
}
