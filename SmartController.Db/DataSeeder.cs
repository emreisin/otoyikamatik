using Microsoft.EntityFrameworkCore;
using SmartController.Db.Entities;
using SmartController.Shared.Enums;

namespace SmartController.Db;

public static class DataSeeder
{
    public static async Task SeedAsync(SmartControllerDbContext db)
    {
        if (await db.Distributors.AnyAsync()) return;

        // Distribütörler
        var dist1 = new Distributor { Kod = "DIST01", Ad = "ABC Otoyıkama Sistemleri", Telefon = "0212 555 1234", Email = "info@abc.com" };
        var dist2 = new Distributor { Kod = "DIST02", Ad = "XYZ Otomasyon", Telefon = "0312 444 5678", Email = "info@xyz.com" };
        db.Distributors.AddRange(dist1, dist2);
        await db.SaveChangesAsync();

        // Kullanıcılar
        var adminUser = new User
        {
            Email = "admin@smartcontroller.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Ad = "Sistem",
            Soyad = "Admin",
            Role = UserRole.Admin
        };
        var dist1User = new User
        {
            Email = "abc@smartcontroller.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("abc123"),
            Ad = "ABC",
            Soyad = "Yönetici",
            Role = UserRole.Distributor,
            DistributorId = dist1.Id
        };
        var dist2User = new User
        {
            Email = "xyz@smartcontroller.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("xyz123"),
            Ad = "XYZ",
            Soyad = "Yönetici",
            Role = UserRole.Distributor,
            DistributorId = dist2.Id
        };
        db.Users.AddRange(adminUser, dist1User, dist2User);
        await db.SaveChangesAsync();

        // Şubeler
        var sube1 = new Sube { DistributorId = dist1.Id, Kod = "IST01", Ad = "İstanbul Merkez", Telefon = "0212 111 2233" };
        var sube2 = new Sube { DistributorId = dist1.Id, Kod = "IST02", Ad = "İstanbul Kadıköy", Telefon = "0216 222 3344" };
        var sube3 = new Sube { DistributorId = dist2.Id, Kod = "ANK01", Ad = "Ankara Çankaya", Telefon = "0312 333 4455" };
        db.Subeler.AddRange(sube1, sube2, sube3);
        await db.SaveChangesAsync();

        // Peronlar
        var peron1 = new Peron { SubeId = sube1.Id, PeronNo = 1, Aktif = true };
        var peron2 = new Peron { SubeId = sube1.Id, PeronNo = 2, Aktif = true };
        var peron3 = new Peron { SubeId = sube2.Id, PeronNo = 1, Aktif = true };
        var peron4 = new Peron { SubeId = sube3.Id, PeronNo = 1, Aktif = true };
        var peron5 = new Peron { SubeId = sube3.Id, PeronNo = 2, Aktif = true };
        db.Peronlar.AddRange(peron1, peron2, peron3, peron4, peron5);
        await db.SaveChangesAsync();

        // Cihazlar
        var device1 = new Device { DeviceId = "SCE05A1BA0FE94", DeviceName = "SmartController-01", SubeId = sube1.Id, PeronId = peron1.Id, FirmwareVersion = "2.1.1", IsOnline = true, LastSeen = DateTime.UtcNow };
        var device2 = new Device { DeviceId = "SCE05A1BA0FE95", DeviceName = "SmartController-02", SubeId = sube1.Id, PeronId = peron2.Id, FirmwareVersion = "2.1.0", IsOnline = false, LastSeen = DateTime.UtcNow.AddHours(-5) };
        var device3 = new Device { DeviceId = "SCE05A1BA0FE96", DeviceName = "SmartController-03", SubeId = sube2.Id, PeronId = peron3.Id, FirmwareVersion = "2.1.1", IsOnline = true, LastSeen = DateTime.UtcNow.AddMinutes(-10) };
        var device4 = new Device { DeviceId = "SCE05A1BA0FE97", DeviceName = "SmartController-04", SubeId = sube3.Id, PeronId = peron4.Id, FirmwareVersion = "2.1.1", IsOnline = true, LastSeen = DateTime.UtcNow.AddMinutes(-5) };
        var device5 = new Device { DeviceId = "SCE05A1BA0FE98", DeviceName = "SmartController-05", SubeId = sube3.Id, PeronId = peron5.Id, FirmwareVersion = "2.0.0", IsOnline = true, LastSeen = DateTime.UtcNow.AddMinutes(-2) };
        db.Devices.AddRange(device1, device2, device3, device4, device5);
        await db.SaveChangesAsync();

        // Sayaçlar (Ödeme Kanalları: 0=Dijital, 1=Jeton, 2=Kart, 3=Nakit, 4=QRKod)
        db.DeviceCounters.AddRange(
            new DeviceCounter { DeviceId = device1.DeviceId, CounterType = CounterType.Dijital, TotalCount = 450, LastIncrementAt = DateTime.UtcNow },
            new DeviceCounter { DeviceId = device1.DeviceId, CounterType = CounterType.Jeton, TotalCount = 320, LastIncrementAt = DateTime.UtcNow },
            new DeviceCounter { DeviceId = device1.DeviceId, CounterType = CounterType.Kart, TotalCount = 280, LastIncrementAt = DateTime.UtcNow },
            new DeviceCounter { DeviceId = device1.DeviceId, CounterType = CounterType.Nakit, TotalCount = 150, LastIncrementAt = DateTime.UtcNow },
            new DeviceCounter { DeviceId = device1.DeviceId, CounterType = CounterType.QRKod, TotalCount = 50, LastIncrementAt = DateTime.UtcNow },
            new DeviceCounter { DeviceId = device2.DeviceId, CounterType = CounterType.Jeton, TotalCount = 500, LastIncrementAt = DateTime.UtcNow.AddHours(-5) },
            new DeviceCounter { DeviceId = device2.DeviceId, CounterType = CounterType.Nakit, TotalCount = 250, LastIncrementAt = DateTime.UtcNow.AddHours(-5) },
            new DeviceCounter { DeviceId = device3.DeviceId, CounterType = CounterType.Kart, TotalCount = 1200, LastIncrementAt = DateTime.UtcNow },
            new DeviceCounter { DeviceId = device3.DeviceId, CounterType = CounterType.QRKod, TotalCount = 900, LastIncrementAt = DateTime.UtcNow },
            new DeviceCounter { DeviceId = device4.DeviceId, CounterType = CounterType.Dijital, TotalCount = 800, LastIncrementAt = DateTime.UtcNow },
            new DeviceCounter { DeviceId = device4.DeviceId, CounterType = CounterType.Jeton, TotalCount = 1400, LastIncrementAt = DateTime.UtcNow },
            new DeviceCounter { DeviceId = device5.DeviceId, CounterType = CounterType.Nakit, TotalCount = 600, LastIncrementAt = DateTime.UtcNow },
            new DeviceCounter { DeviceId = device5.DeviceId, CounterType = CounterType.Kart, TotalCount = 1200, LastIncrementAt = DateTime.UtcNow }
        );

        // Status Logları
        db.DeviceStatusLogs.AddRange(
            new DeviceStatusLog { DeviceId = device1.DeviceId, WifiSsid = "CarWash-WiFi", WifiSignal = -45, IpAddress = "192.168.1.101", Uptime = "05:23:45" },
            new DeviceStatusLog { DeviceId = device2.DeviceId, WifiSsid = "CarWash-WiFi", WifiSignal = -72, IpAddress = "192.168.1.102", Uptime = "01:10:00", CreatedAt = DateTime.UtcNow.AddHours(-5) },
            new DeviceStatusLog { DeviceId = device3.DeviceId, WifiSsid = "Kadikoy-WiFi", WifiSignal = -55, IpAddress = "192.168.2.101", Uptime = "12:45:00" },
            new DeviceStatusLog { DeviceId = device4.DeviceId, WifiSsid = "Ankara-WiFi", WifiSignal = -48, IpAddress = "192.168.3.101", Uptime = "08:30:00" },
            new DeviceStatusLog { DeviceId = device5.DeviceId, WifiSsid = "Ankara-WiFi", WifiSignal = -52, IpAddress = "192.168.3.102", Uptime = "06:15:00" }
        );

        // Firmware
        db.FirmwareVersions.AddRange(
            new FirmwareVersion { Version = "2.0.0", FirmwareUrl = "https://firmware.example.com/v2.0.0.bin", Mandatory = false, ReleaseDate = DateTime.UtcNow.AddMonths(-3) },
            new FirmwareVersion { Version = "2.1.0", FirmwareUrl = "https://firmware.example.com/v2.1.0.bin", Mandatory = false, ReleaseDate = DateTime.UtcNow.AddMonths(-1) },
            new FirmwareVersion { Version = "2.1.1", FirmwareUrl = "https://firmware.example.com/v2.1.1.bin", Mandatory = true, ReleaseDate = DateTime.UtcNow.AddDays(-7) }
        );

        // Komutlar
        db.Commands.AddRange(
            new Command { DeviceId = device1.DeviceId, CommandText = "{DATACHECK}", Status = CommandStatus.Delivered, DeliveredAt = DateTime.UtcNow.AddMinutes(-30), CreatedAt = DateTime.UtcNow.AddMinutes(-35) },
            new Command { DeviceId = device2.DeviceId, CommandText = "{UPDATE}", Status = CommandStatus.Pending, CreatedAt = DateTime.UtcNow.AddHours(-2) }
        );

        await db.SaveChangesAsync();
    }
}
