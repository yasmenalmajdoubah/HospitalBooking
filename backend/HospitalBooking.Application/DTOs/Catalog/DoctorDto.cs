namespace HospitalBooking.Application.DTOs.Catalog;

public class DoctorDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public string SpecialtyAr { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string DepartmentNameAr { get; set; } = string.Empty;
}
