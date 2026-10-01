namespace HospitalBooking.Domain.Entities;

/// <summary>
/// Table: Users — حسابات المستخدمين.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
    public string? Phone { get; set; }
    public int RoleId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Role Role { get; set; } = null!;
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
    public ICollection<Appointment> CreatedAppointments { get; set; } = new List<Appointment>();
}
