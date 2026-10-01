namespace HospitalBooking.Application.DTOs.Catalog;

public class PatientListItemDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
    public string? Phone { get; set; }
    public string? NationalId { get; set; }
}
