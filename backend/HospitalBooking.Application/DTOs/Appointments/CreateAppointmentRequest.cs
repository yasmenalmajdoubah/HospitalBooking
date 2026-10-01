using System.ComponentModel.DataAnnotations;

namespace HospitalBooking.Application.DTOs.Appointments;

public class CreateAppointmentRequest
{
    /// <summary>
    /// Required for Admin/Receptionist. Ignored for Patient (uses own profile).
    /// </summary>
    public int? PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [Required]
    public DateOnly AppointmentDate { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Optional. Defaults to doctor's schedule slot or 30 minutes.
    /// </summary>
    public TimeOnly? EndTime { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
