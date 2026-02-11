using Microsoft.EntityFrameworkCore;
using SmartController.Db.Entities;

namespace SmartController.Db;

public class SmartControllerDbContext : DbContext
{
    public SmartControllerDbContext(DbContextOptions<SmartControllerDbContext> options) : base(options) { }

    public DbSet<Distributor> Distributors => Set<Distributor>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Sube> Subeler => Set<Sube>();
    public DbSet<Peron> Peronlar => Set<Peron>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<DeviceStatusLog> DeviceStatusLogs => Set<DeviceStatusLog>();
    public DbSet<DeviceRawData> DeviceRawData => Set<DeviceRawData>();
    public DbSet<DeviceCounter> DeviceCounters => Set<DeviceCounter>();
    public DbSet<Command> Commands => Set<Command>();
    public DbSet<FirmwareVersion> FirmwareVersions => Set<FirmwareVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(e =>
        {
            e.HasKey(d => d.DeviceId);
            e.Property(d => d.DeviceId).HasMaxLength(50);
            e.HasOne(d => d.Sube).WithMany(s => s.Devices).HasForeignKey(d => d.SubeId);
            e.HasOne(d => d.Peron).WithMany(p => p.Devices).HasForeignKey(d => d.PeronId);
        });

        modelBuilder.Entity<Distributor>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.Kod).HasMaxLength(20);
            e.Property(d => d.Ad).HasMaxLength(100);
            e.HasIndex(d => d.Kod).IsUnique();
        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Email).HasMaxLength(100);
            e.HasIndex(u => u.Email).IsUnique();
            e.HasOne(u => u.Distributor).WithMany(d => d.Users).HasForeignKey(u => u.DistributorId);
        });

        modelBuilder.Entity<Sube>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Kod).HasMaxLength(20);
            e.Property(s => s.Ad).HasMaxLength(100);
            e.HasOne(s => s.Distributor).WithMany(d => d.Subeler).HasForeignKey(s => s.DistributorId);
        });

        modelBuilder.Entity<Peron>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasOne(p => p.Sube).WithMany(s => s.Peronlar).HasForeignKey(p => p.SubeId);
        });

        modelBuilder.Entity<DeviceStatusLog>(e =>
        {
            e.HasKey(l => l.Id);
            e.HasOne(l => l.Device).WithMany(d => d.StatusLogs).HasForeignKey(l => l.DeviceId);
            e.HasIndex(l => l.CreatedAt);
        });

        modelBuilder.Entity<DeviceRawData>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.RawData).HasColumnType("jsonb");
            e.HasOne(r => r.Device).WithMany(d => d.RawDataLogs).HasForeignKey(r => r.DeviceId);
            e.HasIndex(r => r.CreatedAt);
        });

        modelBuilder.Entity<DeviceCounter>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasOne(c => c.Device).WithMany(d => d.Counters).HasForeignKey(c => c.DeviceId);
            e.HasIndex(c => new { c.DeviceId, c.CounterType }).IsUnique();
        });

        modelBuilder.Entity<Command>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasOne(c => c.Device).WithMany(d => d.Commands).HasForeignKey(c => c.DeviceId);
            e.HasIndex(c => new { c.DeviceId, c.Status });
        });

        modelBuilder.Entity<FirmwareVersion>(e =>
        {
            e.HasKey(f => f.Id);
            e.Property(f => f.Version).HasMaxLength(20);
            e.HasIndex(f => f.Version).IsUnique();
        });
    }
}
