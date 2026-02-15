using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartController.Db;
using SmartController.Shared.DTOs;
using SmartController.Shared.Enums;

namespace SmartController.Api.Controllers;

[ApiController]
[Route("commands")]
public class CommandsController : ControllerBase
{
    private readonly SmartControllerDbContext _db;

    public CommandsController(SmartControllerDbContext db) => _db = db;

    [HttpGet("{customerSuffix}")]
    public async Task<IActionResult> GetCommand(string customerSuffix, [FromQuery] int peron_id, [FromQuery] string sube)
    {
        // Distributor doğrula
        var distributor = await _db.Distributors.FirstOrDefaultAsync(d => d.Kod == customerSuffix);
        if (distributor == null)
            return BadRequest(new { error = "Invalid customerSuffix" });

        var device = await _db.Devices
            .Include(d => d.Peron)
            .Include(d => d.Sube)
            .FirstOrDefaultAsync(d => d.Peron.PeronNo == peron_id && d.Sube.Kod == sube && d.Sube.DistributorId == distributor.Id);

        if (device == null)
            return Ok(new CommandResponseDto { Command = "" });

        // Cihazın online durumunu güncelle
        device.LastSeen = DateTime.UtcNow;
        device.IsOnline = true;

        var command = await _db.Commands
            .Where(c => c.DeviceId == device.DeviceId && c.Status == CommandStatus.Pending)
            .OrderBy(c => c.CreatedAt)
            .FirstOrDefaultAsync();

        if (command == null)
        {
            await _db.SaveChangesAsync();
            return Ok(new CommandResponseDto { Command = "" });
        }

        // Komut bulundu, ID'yi de dön (ack için)
        await _db.SaveChangesAsync();
        return Ok(new { command = command.CommandText, commandId = command.Id });
    }

    [HttpPost("{customerSuffix}/ack")]
    public async Task<IActionResult> AckCommand(string customerSuffix, [FromBody] CommandAckDto dto)
    {
        var command = await _db.Commands.FindAsync(dto.CommandId);
        if (command != null)
        {
            command.Status = dto.Success ? CommandStatus.Delivered : CommandStatus.Failed;
            command.DeliveredAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return Ok(new { success = true });
    }
}
