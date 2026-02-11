namespace SmartController.Db.Entities;

public class Distributor
{
    public int Id { get; set; }
    public string Kod { get; set; } = string.Empty;
    public string Ad { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? Adres { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Sube> Subeler { get; set; } = new List<Sube>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
