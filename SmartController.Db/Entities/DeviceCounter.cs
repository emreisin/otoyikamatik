using SmartController.Shared.Enums;

namespace SmartController.Db.Entities;

public class DeviceCounter
{
    public long Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public CounterType CounterType { get; set; }
    public long TotalCount { get; set; }
    public DateTime? LastIncrementAt { get; set; }
    
    public Device Device { get; set; } = null!;
}
