using System.ComponentModel.DataAnnotations;

namespace HospitalBooking.Application.DTOs.Appointments;

public class UpdateAppointmentRequest
{
    [Required]
    public DateOnly AppointmentDate { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
