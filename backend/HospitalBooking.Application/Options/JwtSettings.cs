namespace HospitalBooking.Application.Options;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "HospitalBooking";
    public string Audience { get; set; } = "HospitalBooking.Client";
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 120;
}
