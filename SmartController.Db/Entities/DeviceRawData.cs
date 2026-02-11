namespace SmartController.Db.Entities;

public class DeviceRawData
{
    public long Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string RawData { get; set; } = string.Empty;
    public bool Parsed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Device Device { get; set; } = null!;
}
