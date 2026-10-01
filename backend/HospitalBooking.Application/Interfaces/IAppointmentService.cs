using HospitalBooking.Application.DTOs.Appointments;

namespace HospitalBooking.Application.Interfaces;

public interface IAppointmentService
{
    Task<IReadOnlyList<AppointmentDto>> GetAsync(
        AppointmentFilter filter,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken = default);

    Task<AppointmentDto?> GetByIdAsync(
        int id,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken = default);

    Task<AppointmentDto> CreateAsync(
        CreateAppointmentRequest request,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken = default);

    Task<AppointmentDto> UpdateAsync(
        int id,
        UpdateAppointmentRequest request,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken = default);

    Task<AppointmentDto> UpdateStatusAsync(
        int id,
        UpdateAppointmentStatusRequest request,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken = default);

    Task<AppointmentDto> CancelAsync(
        int id,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken = default);
}
