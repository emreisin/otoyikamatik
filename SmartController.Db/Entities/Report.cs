using SmartController.Shared.Enums;

namespace SmartController.Db.Entities;

public class Report
{
    public long Id { get; set; }
    public int SubeId { get; set; }
    public ReportType ReportType { get; set; }
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;
    public int CreatedByUserId { get; set; }
    public bool IsAutomatic { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Sube Sube { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
    public ICollection<ReportDetail> Details { get; set; } = new List<ReportDetail>();
}
