using HospitalBooking.Application.DTOs.Catalog;
using HospitalBooking.Application.Interfaces;
using HospitalBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HospitalBooking.Infrastructure.Services;

public class CatalogService : ICatalogService
{
    private readonly HospitalBookingDbContext _dbContext;

    public CatalogService(HospitalBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Departments
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new DepartmentDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAr = x.NameAr,
                Description = x.Description
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorDto>> GetDoctorsAsync(int? departmentId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Doctors
            .AsNoTracking()
            .Where(x => x.User.IsActive && x.Department.IsActive);

        if (departmentId.HasValue)
        {
            query = query.Where(x => x.DepartmentId == departmentId.Value);
        }

        return await query
            .OrderBy(x => x.User.FullName)
            .Select(x => new DoctorDto
            {
                Id = x.Id,
                FullName = x.User.FullName,
                FullNameAr = x.User.FullNameAr,
                Specialty = x.Specialty,
                SpecialtyAr = x.SpecialtyAr,
                DepartmentId = x.DepartmentId,
                DepartmentName = x.Department.Name,
                DepartmentNameAr = x.Department.NameAr
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentStatusDto>> GetAppointmentStatusesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.AppointmentStatuses
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new AppointmentStatusDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAr = x.NameAr
            })
            .ToListAsync(cancellationToken);
    }
}
