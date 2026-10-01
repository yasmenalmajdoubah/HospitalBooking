using System.ComponentModel.DataAnnotations;

namespace HospitalBooking.Application.DTOs.Appointments;

public class UpdateAppointmentStatusRequest
{
    [Required]
    public int StatusId { get; set; }
}
