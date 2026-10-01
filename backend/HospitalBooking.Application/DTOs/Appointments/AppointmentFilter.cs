namespace HospitalBooking.Application.DTOs.Appointments;

public class AppointmentFilter
{
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public int? DoctorId { get; set; }
    public int? PatientId { get; set; }
    public int? StatusId { get; set; }
    public int? DepartmentId { get; set; }
}
