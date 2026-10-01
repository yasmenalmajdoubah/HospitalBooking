using System.ComponentModel.DataAnnotations;

namespace HospitalBooking.Application.DTOs.Catalog;

public class CreateDepartmentRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string NameAr { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
