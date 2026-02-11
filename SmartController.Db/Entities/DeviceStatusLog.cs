namespace SmartController.Db.Entities;

public class DeviceStatusLog
{
    public long Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string WifiSsid { get; set; } = string.Empty;
    public int WifiSignal { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string Uptime { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Device Device { get; set; } = null!;
}
