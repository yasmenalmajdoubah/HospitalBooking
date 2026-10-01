namespace HospitalBooking.Application.DTOs.Auth;

public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
    public string Role { get; set; } = string.Empty;
    public string RoleAr { get; set; } = string.Empty;
}
