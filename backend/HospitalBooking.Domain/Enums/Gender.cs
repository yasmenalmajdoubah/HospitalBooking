namespace HospitalBooking.Domain.Enums;

/// <summary>
/// Patient gender / جنس المريض.
/// Stored as string in database (nvarchar).
/// </summary>
public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3
}
