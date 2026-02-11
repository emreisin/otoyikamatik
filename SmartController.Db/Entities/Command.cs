using SmartController.Shared.Enums;

namespace SmartController.Db.Entities;

public class Command
{
    public long Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string CommandText { get; set; } = string.Empty;
    public CommandStatus Status { get; set; } = CommandStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAt { get; set; }
    
    public Device Device { get; set; } = null!;
}
