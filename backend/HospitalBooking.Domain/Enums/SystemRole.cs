namespace HospitalBooking.Domain.Enums;

/// <summary>
/// System roles — يجب أن تطابق قيم جدول Roles عند الـ Seed.
/// أدوار النظام.
/// </summary>
public enum SystemRole
{
    Admin = 1,
    Doctor = 2,
    Receptionist = 3,
    Patient = 4
}
