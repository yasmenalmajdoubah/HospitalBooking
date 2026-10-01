using System.ComponentModel.DataAnnotations;

namespace HospitalBooking.Application.DTOs.Auth;

public class RegisterPatientRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? FullNameAr { get; set; }

    [MaxLength(30)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? NationalId { get; set; }
}
