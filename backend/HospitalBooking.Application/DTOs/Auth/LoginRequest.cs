using System.ComponentModel.DataAnnotations;

namespace HospitalBooking.Application.DTOs.Auth;

public class LoginRequest
{
    [Required]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
