using HospitalBooking.Domain.Enums;

namespace HospitalBooking.Domain.Entities;

/// <summary>
/// Table: Patients — المرضى.
/// </summary>
public class Patient
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? NationalId { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }

    public User? User { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
