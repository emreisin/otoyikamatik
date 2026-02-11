namespace SmartController.Db.Entities;

public class Device
{
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public int SubeId { get; set; }
    public int PeronId { get; set; }
    public string FirmwareVersion { get; set; } = string.Empty;
    public DateTime? LastSeen { get; set; }
    public bool IsOnline { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Sube Sube { get; set; } = null!;
    public Peron Peron { get; set; } = null!;
    public ICollection<DeviceStatusLog> StatusLogs { get; set; } = new List<DeviceStatusLog>();
    public ICollection<DeviceRawData> RawDataLogs { get; set; } = new List<DeviceRawData>();
    public ICollection<DeviceCounter> Counters { get; set; } = new List<DeviceCounter>();
    public ICollection<Command> Commands { get; set; } = new List<Command>();
}
