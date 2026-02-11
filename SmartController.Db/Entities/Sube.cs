namespace SmartController.Db.Entities;

public class Sube
{
    public int Id { get; set; }
    public int DistributorId { get; set; }
    public string Kod { get; set; } = string.Empty;
    public string Ad { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public string? Adres { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Distributor Distributor { get; set; } = null!;
    public ICollection<Peron> Peronlar { get; set; } = new List<Peron>();
    public ICollection<Device> Devices { get; set; } = new List<Device>();
}
