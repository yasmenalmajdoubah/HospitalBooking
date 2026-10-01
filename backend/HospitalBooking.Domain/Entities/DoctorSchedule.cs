namespace HospitalBooking.Domain.Entities;

/// <summary>
/// Table: DoctorSchedules — جداول دوام الأطباء.
/// DayOfWeek: 0 = Sunday … 6 = Saturday (System.DayOfWeek).
/// </summary>
public class DoctorSchedule
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SlotDurationMinutes { get; set; } = 30;
    public bool IsActive { get; set; } = true;

    public Doctor Doctor { get; set; } = null!;
}
