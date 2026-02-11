namespace SmartController.Shared.DTOs;

public class DeviceStatusDto
{
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string Sube { get; set; } = string.Empty;
    public int PeronId { get; set; }
    public string FirmwareVersion { get; set; } = string.Empty;
    public string WifiSsid { get; set; } = string.Empty;
    public string WifiPassword { get; set; } = string.Empty;
    public int WifiSignal { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string Uptime { get; set; } = string.Empty;
}
