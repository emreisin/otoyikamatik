namespace SmartController.Shared.DTOs;

public class SendCommandDto
{
    public string DeviceId { get; set; } = string.Empty;
    public string CommandText { get; set; } = string.Empty;
}
