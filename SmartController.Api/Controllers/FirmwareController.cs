using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartController.Db;
using SmartController.Shared.DTOs;

namespace SmartController.Api.Controllers;

[ApiController]
public class FirmwareController : ControllerBase
{
    private readonly SmartControllerDbContext _db;

    public FirmwareController(SmartControllerDbContext db) => _db = db;

    [HttpGet("check_firmware")]
    public async Task<IActionResult> CheckFirmware([FromQuery] string customer, [FromQuery] string device, [FromQuery] string current_version)
    {
        var latest = await _db.FirmwareVersions
            .OrderByDescending(f => f.ReleaseDate)
            .FirstOrDefaultAsync();

        if (latest == null || latest.Version == current_version)
        {
            return Ok(new FirmwareCheckResponseDto
            {
                LatestVersion = current_version,
                UpdateAvailable = false
            });
        }

        return Ok(new FirmwareCheckResponseDto
        {
            LatestVersion = latest.Version,
            FirmwareUrl = latest.FirmwareUrl,
            UpdateAvailable = true,
            Mandatory = latest.Mandatory
        });
    }
}
