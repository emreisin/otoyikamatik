namespace SmartController.Shared.DTOs;

public class CommandAckDto
{
    public long CommandId { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public bool Success { get; set; }
}
