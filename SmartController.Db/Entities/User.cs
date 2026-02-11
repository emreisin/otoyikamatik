using SmartController.Shared.Enums;

namespace SmartController.Db.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public int? DistributorId { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    public Distributor? Distributor { get; set; }
}
