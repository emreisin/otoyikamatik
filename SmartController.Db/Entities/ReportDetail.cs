using SmartController.Shared.Enums;

namespace SmartController.Db.Entities;

public class ReportDetail
{
    public long Id { get; set; }
    public long ReportId { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public CounterType CounterType { get; set; }
    public long CountValue { get; set; }

    public Report Report { get; set; } = null!;
    public Device Device { get; set; } = null!;
}
