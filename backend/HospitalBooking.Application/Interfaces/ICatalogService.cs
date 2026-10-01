using HospitalBooking.Application.DTOs.Catalog;

namespace HospitalBooking.Application.Interfaces;

public interface ICatalogService
{
    Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoctorDto>> GetDoctorsAsync(int? departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppointmentStatusDto>> GetAppointmentStatusesAsync(CancellationToken cancellationToken = default);
}
