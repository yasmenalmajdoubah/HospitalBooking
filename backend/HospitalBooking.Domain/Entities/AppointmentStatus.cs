namespace HospitalBooking.Domain.Entities;

/// <summary>
/// Table: AppointmentStatuses — حالات الحجز (lookup).
/// </summary>
public class AppointmentStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
