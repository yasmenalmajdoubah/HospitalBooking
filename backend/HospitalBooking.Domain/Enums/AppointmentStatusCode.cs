namespace HospitalBooking.Domain.Enums;

/// <summary>
/// Appointment statuses — يجب أن تطابق قيم جدول AppointmentStatuses عند الـ Seed.
/// حالات الحجز.
/// </summary>
public enum AppointmentStatusCode
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3,
    Completed = 4
}
