using System.ComponentModel.DataAnnotations;

namespace HospitalBooking.Application.DTOs.Catalog;

public class CreateDoctorRequest
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

    [Required]
    [Range(1, int.MaxValue)]
    public int DepartmentId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Specialty { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string SpecialtyAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;
}
