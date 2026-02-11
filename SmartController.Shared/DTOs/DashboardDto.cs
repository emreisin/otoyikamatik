namespace SmartController.Shared.DTOs;

public class DashboardDto
{
    public int TotalDevices { get; set; }
    public int OnlineDevices { get; set; }
    public int OfflineDevices { get; set; }
    public int TodayOperations { get; set; }
    public int Last24HoursAlarms { get; set; }
}
