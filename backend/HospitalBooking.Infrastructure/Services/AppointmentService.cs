using HospitalBooking.Application.DTOs.Appointments;
using HospitalBooking.Application.Exceptions;
using HospitalBooking.Application.Interfaces;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Enums;
using HospitalBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HospitalBooking.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly HospitalBookingDbContext _dbContext;

    public AppointmentService(HospitalBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<AppointmentDto>> GetAsync(
        AppointmentFilter filter,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken = default)
    {
        var query = BuildBaseQuery();

        query = await ApplyRoleScopeAsync(query, currentUserId, currentRole, cancellationToken);

        if (filter.FromDate.HasValue)
        {
            query = query.Where(x => x.AppointmentDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(x => x.AppointmentDate <= filter.ToDate.Value);
        }

        if (filter.DoctorId.HasValue && IsStaff(currentRole))
        {
            query = query.Where(x => x.DoctorId == filter.DoctorId.Value);
        }

        if (filter.PatientId.HasValue && IsStaff(currentRole))
        {
            query = query.Where(x => x.PatientId == filter.PatientId.Value);
        }

        if (filter.StatusId.HasValue)
        {
            query = query.Where(x => x.StatusId == filter.StatusId.Value);
        }

        if (filter.DepartmentId.HasValue && IsStaff(currentRole))
        {
            query = query.Where(x => x.Doctor.DepartmentId == filter.DepartmentId.Value);
        }

        var items = await query
            .OrderBy(x => x.AppointmentDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync(cancellationToken);

        return items.Select(Map).ToList();
    }

    public async Task<AppointmentDto?> GetByIdAsync(
        int id,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken = default)
    {
        var query = BuildBaseQuery().Where(x => x.Id == id);
        query = await ApplyRoleScopeAsync(query, currentUserId, currentRole, cancellationToken);

        var appointment = await query.FirstOrDefaultAsync(cancellationToken);
        return appointment is null ? null : Map(appointment);
    }

    public async Task<AppointmentDto> CreateAsync(
        CreateAppointmentRequest request,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken = default)
    {
        if (request.AppointmentDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
        {
            throw new AppException("Cannot book an appointment in the past. / لا يمكن الحجز بتاريخ ماضٍ.", 400);
        }

        var patientId = await ResolvePatientIdAsync(request.PatientId, currentUserId, currentRole, cancellationToken);

        var doctor = await _dbContext.Doctors
            .Include(x => x.User)
            .Include(x => x.Department)
            .Include(x => x.Schedules)
            .FirstOrDefaultAsync(x => x.Id == request.DoctorId && x.User.IsActive && x.Department.IsActive, cancellationToken);

        if (doctor is null)
        {
            throw new AppException("Doctor not found or inactive. / الطبيب غير موجود أو غير مفعّل.", 404);
        }

        var endTime = request.EndTime ?? await ResolveEndTimeAsync(doctor, request.AppointmentDate, request.StartTime, cancellationToken);

        if (endTime <= request.StartTime)
        {
            throw new AppException("EndTime must be after StartTime. / وقت النهاية يجب أن يكون بعد البداية.", 400);
        }

        await EnsureNoConflictAsync(doctor.Id, patientId, request.AppointmentDate, request.StartTime, endTime, excludeId: null, cancellationToken);

        var appointment = new Appointment
        {
            PatientId = patientId,
            DoctorId = doctor.Id,
            AppointmentDate = request.AppointmentDate,
            StartTime = request.StartTime,
            EndTime = endTime,
            StatusId = (int)AppointmentStatusCode.Pending,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            CreatedByUserId = currentUserId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Appointments.Add(appointment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(appointment.Id, currentUserId, currentRole, cancellationToken))!;
    }

    public async Task<AppointmentDto> UpdateAsync(
        int id,
        UpdateAppointmentRequest request,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken = default)
    {
        EnsureStaffCanManage(currentRole);

        var appointment = await _dbContext.Appointments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException("Appointment not found. / الحجز غير موجود.", 404);

        if (appointment.StatusId is (int)AppointmentStatusCode.Cancelled or (int)AppointmentStatusCode.Completed)
        {
            throw new AppException("Cannot update a cancelled/completed appointment. / لا يمكن تعديل حجز ملغي أو مكتمل.", 400);
        }

        if (request.EndTime <= request.StartTime)
        {
            throw new AppException("EndTime must be after StartTime. / وقت النهاية يجب أن يكون بعد البداية.", 400);
        }

        await EnsureNoConflictAsync(
            appointment.DoctorId,
            appointment.PatientId,
            request.AppointmentDate,
            request.StartTime,
            request.EndTime,
            excludeId: appointment.Id,
            cancellationToken);

        appointment.AppointmentDate = request.AppointmentDate;
        appointment.StartTime = request.StartTime;
        appointment.EndTime = request.EndTime;
        appointment.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        appointment.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(appointment.Id, currentUserId, currentRole, cancellationToken))!;
    }

    public async Task<AppointmentDto> UpdateStatusAsync(
        int id,
        UpdateAppointmentStatusRequest request,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(typeof(AppointmentStatusCode), request.StatusId))
        {
            throw new AppException("Invalid status. / حالة الحجز غير صحيحة.", 400);
        }

        var appointment = await FindAccessibleAppointmentAsync(id, currentUserId, currentRole, cancellationToken);

        EnsureCanChangeStatus(currentRole, appointment.StatusId, request.StatusId);

        appointment.StatusId = request.StatusId;
        appointment.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(appointment.Id, currentUserId, currentRole, cancellationToken))!;
    }

    public async Task<AppointmentDto> CancelAsync(
        int id,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken = default)
    {
        return await UpdateStatusAsync(
            id,
            new UpdateAppointmentStatusRequest { StatusId = (int)AppointmentStatusCode.Cancelled },
            currentUserId,
            currentRole,
            cancellationToken);
    }

    private IQueryable<Appointment> BuildBaseQuery()
    {
        return _dbContext.Appointments
            .AsNoTracking()
            .Include(x => x.Status)
            .Include(x => x.Patient).ThenInclude(x => x.User)
            .Include(x => x.Doctor).ThenInclude(x => x.User)
            .Include(x => x.Doctor).ThenInclude(x => x.Department)
            .AsQueryable();
    }

    private async Task<IQueryable<Appointment>> ApplyRoleScopeAsync(
        IQueryable<Appointment> query,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken)
    {
        if (currentRole is "Admin" or "Receptionist")
        {
            return query;
        }

        if (currentRole == "Doctor")
        {
            var doctorId = await _dbContext.Doctors
                .Where(x => x.UserId == currentUserId)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (doctorId is null)
            {
                throw new AppException("Doctor profile not found for current user. / لا يوجد ملف طبيب لهذا المستخدم.", 403);
            }

            return query.Where(x => x.DoctorId == doctorId.Value);
        }

        if (currentRole == "Patient")
        {
            var patientId = await _dbContext.Patients
                .Where(x => x.UserId == currentUserId)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (patientId is null)
            {
                throw new AppException("Patient profile not found for current user. / لا يوجد ملف مريض لهذا المستخدم.", 403);
            }

            return query.Where(x => x.PatientId == patientId.Value);
        }

        throw new AppException("Unsupported role. / دور غير مدعوم.", 403);
    }

    private async Task<Appointment> FindAccessibleAppointmentAsync(
        int id,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Appointments.Where(x => x.Id == id);
        query = await ApplyRoleScopeAsync(query, currentUserId, currentRole, cancellationToken);

        return await query.FirstOrDefaultAsync(cancellationToken)
            ?? throw new AppException("Appointment not found. / الحجز غير موجود.", 404);
    }

    private async Task<int> ResolvePatientIdAsync(
        int? requestedPatientId,
        int currentUserId,
        string currentRole,
        CancellationToken cancellationToken)
    {
        if (currentRole == "Patient")
        {
            var patientId = await _dbContext.Patients
                .Where(x => x.UserId == currentUserId)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (patientId is null)
            {
                throw new AppException("Patient profile not found. / ملف المريض غير موجود.", 403);
            }

            return patientId.Value;
        }

        if (currentRole is not ("Admin" or "Receptionist"))
        {
            throw new AppException("Only Admin/Receptionist/Patient can create appointments. / غير مصرح بإنشاء حجز.", 403);
        }

        if (!requestedPatientId.HasValue)
        {
            throw new AppException("PatientId is required. / رقم المريض مطلوب.", 400);
        }

        var exists = await _dbContext.Patients.AnyAsync(x => x.Id == requestedPatientId.Value, cancellationToken);
        if (!exists)
        {
            throw new AppException("Patient not found. / المريض غير موجود.", 404);
        }

        return requestedPatientId.Value;
    }

    private async Task EnsureNoConflictAsync(
        int doctorId,
        int patientId,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        var doctorConflict = await _dbContext.Appointments.AnyAsync(
            x => x.DoctorId == doctorId
                 && x.AppointmentDate == date
                 && x.StatusId != (int)AppointmentStatusCode.Cancelled
                 && (!excludeId.HasValue || x.Id != excludeId.Value)
                 && x.StartTime < end
                 && start < x.EndTime,
            cancellationToken);

        if (doctorConflict)
        {
            throw new AppException("Doctor already has an appointment in this time slot. / الطبيب لديه موعد في نفس الوقت.", 409);
        }

        var patientConflict = await _dbContext.Appointments.AnyAsync(
            x => x.PatientId == patientId
                 && x.AppointmentDate == date
                 && x.StatusId != (int)AppointmentStatusCode.Cancelled
                 && (!excludeId.HasValue || x.Id != excludeId.Value)
                 && x.StartTime < end
                 && start < x.EndTime,
            cancellationToken);

        if (patientConflict)
        {
            throw new AppException("Patient already has an appointment in this time slot. / المريض لديه موعد في نفس الوقت.", 409);
        }
    }

    private Task<TimeOnly> ResolveEndTimeAsync(
        Doctor doctor,
        DateOnly date,
        TimeOnly startTime,
        CancellationToken cancellationToken)
    {
        var day = date.DayOfWeek;
        var schedule = doctor.Schedules.FirstOrDefault(x => x.IsActive && x.DayOfWeek == day);
        var minutes = schedule?.SlotDurationMinutes > 0 ? schedule.SlotDurationMinutes : 30;
        return Task.FromResult(startTime.AddMinutes(minutes));
    }

    private static void EnsureStaffCanManage(string currentRole)
    {
        if (currentRole is not ("Admin" or "Receptionist"))
        {
            throw new AppException("Only Admin/Receptionist can update appointment details. / غير مصرح بتعديل تفاصيل الحجز.", 403);
        }
    }

    private static void EnsureCanChangeStatus(string currentRole, int currentStatusId, int newStatusId)
    {
        if (currentStatusId == (int)AppointmentStatusCode.Cancelled && newStatusId != (int)AppointmentStatusCode.Cancelled)
        {
            throw new AppException("Cancelled appointments cannot be reopened here. / لا يمكن إعادة فتح حجز ملغي من هنا.", 400);
        }

        if (currentRole == "Patient")
        {
            if (newStatusId != (int)AppointmentStatusCode.Cancelled)
            {
                throw new AppException("Patients can only cancel appointments. / المريض يمكنه إلغاء الحجز فقط.", 403);
            }

            return;
        }

        if (currentRole == "Doctor")
        {
            if (newStatusId is not (
                (int)AppointmentStatusCode.Confirmed
                or (int)AppointmentStatusCode.Completed
                or (int)AppointmentStatusCode.Cancelled))
            {
                throw new AppException("Doctor can set Confirmed/Completed/Cancelled only. / الطبيب يستطيع تأكيد/إكمال/إلغاء فقط.", 403);
            }

            return;
        }

        if (currentRole is not ("Admin" or "Receptionist"))
        {
            throw new AppException("Not allowed to change appointment status. / غير مصرح بتغيير حالة الحجز.", 403);
        }
    }

    private static bool IsStaff(string role) => role is "Admin" or "Receptionist" or "Doctor";

    private static AppointmentDto Map(Appointment x) => new()
    {
        Id = x.Id,
        PatientId = x.PatientId,
        PatientName = x.Patient.User?.FullName ?? $"Patient #{x.PatientId}",
        PatientNameAr = x.Patient.User?.FullNameAr,
        DoctorId = x.DoctorId,
        DoctorName = x.Doctor.User.FullName,
        DoctorNameAr = x.Doctor.User.FullNameAr,
        Specialty = x.Doctor.Specialty,
        SpecialtyAr = x.Doctor.SpecialtyAr,
        DepartmentId = x.Doctor.DepartmentId,
        DepartmentName = x.Doctor.Department.Name,
        DepartmentNameAr = x.Doctor.Department.NameAr,
        AppointmentDate = x.AppointmentDate,
        StartTime = x.StartTime,
        EndTime = x.EndTime,
        StatusId = x.StatusId,
        Status = x.Status.Name,
        StatusAr = x.Status.NameAr,
        Notes = x.Notes,
        CreatedByUserId = x.CreatedByUserId,
        CreatedAt = x.CreatedAt,
        UpdatedAt = x.UpdatedAt
    };
}
