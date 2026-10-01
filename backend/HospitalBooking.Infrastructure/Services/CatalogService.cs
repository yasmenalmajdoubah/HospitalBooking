using HospitalBooking.Application.DTOs.Catalog;
using HospitalBooking.Application.Exceptions;
using HospitalBooking.Application.Interfaces;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Enums;
using HospitalBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HospitalBooking.Infrastructure.Services;

public class CatalogService : ICatalogService
{
    private readonly HospitalBookingDbContext _dbContext;
    private readonly IPasswordHasherService _passwordHasher;

    public CatalogService(HospitalBookingDbContext dbContext, IPasswordHasherService passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
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

        if (departmentId.HasValue && departmentId.Value > 0)
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

    public async Task<IReadOnlyList<PatientListItemDto>> GetPatientsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new PatientListItemDto
            {
                Id = x.Id,
                FullName = x.User != null ? x.User.FullName : $"Patient #{x.Id}",
                FullNameAr = x.User != null ? x.User.FullNameAr : null,
                Phone = x.User != null ? x.User.Phone : null,
                NationalId = x.NationalId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        var nameAr = request.NameAr.Trim();

        var exists = await _dbContext.Departments.AnyAsync(
            x => x.Name == name || x.NameAr == nameAr,
            cancellationToken);

        if (exists)
        {
            throw new AppException("Department already exists. / القسم موجود مسبقاً.", 409);
        }

        var department = new Department
        {
            Name = name,
            NameAr = nameAr,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            IsActive = true
        };

        _dbContext.Departments.Add(department);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            NameAr = department.NameAr,
            Description = department.Description
        };
    }

    public async Task<DoctorDto> CreateDoctorAsync(
        CreateDoctorRequest request,
        CancellationToken cancellationToken = default)
    {
        var department = await _dbContext.Departments
            .FirstOrDefaultAsync(x => x.Id == request.DepartmentId && x.IsActive, cancellationToken);

        if (department is null)
        {
            throw new AppException("Department not found. / القسم غير موجود.", 404);
        }

        var userName = request.UserName.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        var userExists = await _dbContext.Users.AnyAsync(
            x => x.UserName == userName || x.Email == email,
            cancellationToken);

        if (userExists)
        {
            throw new AppException("Username or email already exists. / اسم المستخدم أو البريد مستخدم مسبقاً.", 409);
        }

        var license = request.LicenseNumber.Trim();
        var licenseExists = await _dbContext.Doctors.AnyAsync(x => x.LicenseNumber == license, cancellationToken);
        if (licenseExists)
        {
            throw new AppException("License number already exists. / رقم الترخيص مستخدم مسبقاً.", 409);
        }

        var user = new User
        {
            UserName = userName,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            FullName = request.FullName.Trim(),
            FullNameAr = string.IsNullOrWhiteSpace(request.FullNameAr) ? null : request.FullNameAr.Trim(),
            RoleId = (int)SystemRole.Doctor,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Doctor = new Doctor
            {
                DepartmentId = department.Id,
                Specialty = request.Specialty.Trim(),
                SpecialtyAr = request.SpecialtyAr.Trim(),
                LicenseNumber = license,
                Schedules =
                [
                    new DoctorSchedule
                    {
                        DayOfWeek = DayOfWeek.Sunday,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(14, 0),
                        SlotDurationMinutes = 30,
                        IsActive = true
                    },
                    new DoctorSchedule
                    {
                        DayOfWeek = DayOfWeek.Monday,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(14, 0),
                        SlotDurationMinutes = 30,
                        IsActive = true
                    },
                    new DoctorSchedule
                    {
                        DayOfWeek = DayOfWeek.Tuesday,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(14, 0),
                        SlotDurationMinutes = 30,
                        IsActive = true
                    }
                ]
            }
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DoctorDto
        {
            Id = user.Doctor!.Id,
            FullName = user.FullName,
            FullNameAr = user.FullNameAr,
            Specialty = user.Doctor.Specialty,
            SpecialtyAr = user.Doctor.SpecialtyAr,
            DepartmentId = department.Id,
            DepartmentName = department.Name,
            DepartmentNameAr = department.NameAr
        };
    }
}
