namespace SmartController.Shared.DTOs;

public class FirmwareCheckResponseDto
{
    public string LatestVersion { get; set; } = string.Empty;
    public string FirmwareUrl { get; set; } = string.Empty;
    public bool UpdateAvailable { get; set; }
    public bool Mandatory { get; set; }
}
