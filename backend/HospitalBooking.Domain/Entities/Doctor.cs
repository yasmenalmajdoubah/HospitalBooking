namespace HospitalBooking.Domain.Entities;

/// <summary>
/// Table: Doctors — الأطباء.
/// </summary>
public class Doctor
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public string SpecialtyAr { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;

    public User User { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
