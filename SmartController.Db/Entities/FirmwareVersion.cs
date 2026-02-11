namespace SmartController.Db.Entities;

public class FirmwareVersion
{
    public int Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public string FirmwareUrl { get; set; } = string.Empty;
    public bool Mandatory { get; set; }
    public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;
}
