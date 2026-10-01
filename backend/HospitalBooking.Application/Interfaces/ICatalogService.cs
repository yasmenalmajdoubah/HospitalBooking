using HospitalBooking.Application.DTOs.Catalog;

namespace HospitalBooking.Application.Interfaces;

public interface ICatalogService
{
    Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoctorDto>> GetDoctorsAsync(int? departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppointmentStatusDto>> GetAppointmentStatusesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PatientListItemDto>> GetPatientsAsync(CancellationToken cancellationToken = default);

    Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default);
    Task<DoctorDto> CreateDoctorAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default);
}
