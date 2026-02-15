using SmartController.Shared.Enums;

namespace SmartController.Db.Entities;

public class ScheduledJob
{
    public int Id { get; set; }
    public int SubeId { get; set; }
    public ScheduleFrequency Frequency { get; set; }
    public TimeOnly ExecutionTime { get; set; }  // Saat (örn: 23:00)
    public int? DayOfWeek { get; set; }          // Haftalık için (0=Pazar, 1=Pazartesi...)
    public int? DayOfMonth { get; set; }         // Aylık için (1-28)
    public bool IsActive { get; set; } = true;
    public DateTime? LastExecutedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Sube Sube { get; set; } = null!;
}
