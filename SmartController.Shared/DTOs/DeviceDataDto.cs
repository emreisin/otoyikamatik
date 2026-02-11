namespace SmartController.Shared.DTOs;

public class DeviceDataDto
{
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string RawData { get; set; } = string.Empty;
    public int PeronId { get; set; }
    public string Sube { get; set; } = string.Empty;
}
