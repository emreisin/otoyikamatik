namespace SmartController.Db.Entities;

public class Peron
{
    public int Id { get; set; }
    public int SubeId { get; set; }
    public int PeronNo { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Sube Sube { get; set; } = null!;
    public ICollection<Device> Devices { get; set; } = new List<Device>();
}
