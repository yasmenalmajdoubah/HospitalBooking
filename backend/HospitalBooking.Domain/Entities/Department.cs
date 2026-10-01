namespace HospitalBooking.Domain.Entities;

/// <summary>
/// Table: Departments — أقسام المستشفى.
/// </summary>
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
