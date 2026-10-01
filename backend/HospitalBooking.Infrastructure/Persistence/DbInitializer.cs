using HospitalBooking.Application.Interfaces;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HospitalBooking.Infrastructure.Persistence;

public static class DbInitializer
{
    public const string DefaultAdminUserName = "admin";
    public const string DefaultAdminPassword = "Admin@123";

    public static async Task InitializeAsync(IServiceProvider services, bool applyMigrations)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var db = scope.ServiceProvider.GetRequiredService<HospitalBookingDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();
        var provider = configuration.GetValue<string>("Database:Provider") ?? "Sqlite";

        try
        {
            if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                await db.Database.EnsureCreatedAsync();
                logger.LogInformation("SQLite database ready: HospitalBookingDb.sqlite");
            }
            else if (applyMigrations)
            {
                await db.Database.MigrateAsync();
                logger.LogInformation("Applied EF migrations to HospitalBookingDb (if any were pending).");
            }

            if (!await db.Database.CanConnectAsync())
            {
                logger.LogWarning("Cannot connect to HospitalBookingDb. Skipping seed.");
                return;
            }

            await SeedLookupsAsync(db);
            await SeedAdminAsync(db, passwordHasher, logger);
            await SeedDemoCatalogAsync(db, passwordHasher, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database initialize/seed failed.");
            throw;
        }
    }

    private static async Task SeedLookupsAsync(HospitalBookingDbContext db)
    {
        if (!await db.Roles.AnyAsync())
        {
            db.Roles.AddRange(
                new Role { Id = (int)SystemRole.Admin, Name = "Admin", NameAr = "مدير النظام" },
                new Role { Id = (int)SystemRole.Doctor, Name = "Doctor", NameAr = "طبيب" },
                new Role { Id = (int)SystemRole.Receptionist, Name = "Receptionist", NameAr = "موظف استقبال" },
                new Role { Id = (int)SystemRole.Patient, Name = "Patient", NameAr = "مريض" });
        }

        if (!await db.AppointmentStatuses.AnyAsync())
        {
            db.AppointmentStatuses.AddRange(
                new AppointmentStatus { Id = (int)AppointmentStatusCode.Pending, Name = "Pending", NameAr = "قيد الانتظار" },
                new AppointmentStatus { Id = (int)AppointmentStatusCode.Confirmed, Name = "Confirmed", NameAr = "مؤكد" },
                new AppointmentStatus { Id = (int)AppointmentStatusCode.Cancelled, Name = "Cancelled", NameAr = "ملغي" },
                new AppointmentStatus { Id = (int)AppointmentStatusCode.Completed, Name = "Completed", NameAr = "مكتمل" });
        }

        await db.SaveChangesAsync();
    }

    private static async Task SeedAdminAsync(
        HospitalBookingDbContext db,
        IPasswordHasherService passwordHasher,
        ILogger logger)
    {
        if (await db.Users.AnyAsync(x => x.UserName == DefaultAdminUserName))
        {
            return;
        }

        db.Users.Add(new User
        {
            UserName = DefaultAdminUserName,
            Email = "admin@hospitalbooking.local",
            PasswordHash = passwordHasher.HashPassword(DefaultAdminPassword),
            FullName = "System Administrator",
            FullNameAr = "مدير النظام",
            RoleId = (int)SystemRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
        logger.LogInformation("Seeded admin '{User}' / {Password}", DefaultAdminUserName, DefaultAdminPassword);
    }

    private static async Task SeedDemoCatalogAsync(
        HospitalBookingDbContext db,
        IPasswordHasherService passwordHasher,
        ILogger logger)
    {
        if (!await db.Departments.AnyAsync())
        {
            db.Departments.AddRange(
                new Department { Name = "Cardiology", NameAr = "القلب", Description = "Heart care", IsActive = true },
                new Department { Name = "Orthopedics", NameAr = "العظام", Description = "Bones and joints", IsActive = true },
                new Department { Name = "Pediatrics", NameAr = "الأطفال", Description = "Child care", IsActive = true });
            await db.SaveChangesAsync();
            logger.LogInformation("Seeded demo departments.");
        }

        if (!await db.Doctors.AnyAsync())
        {
            var cardiologyId = await db.Departments.Where(x => x.Name == "Cardiology").Select(x => x.Id).FirstAsync();
            var orthoId = await db.Departments.Where(x => x.Name == "Orthopedics").Select(x => x.Id).FirstAsync();

            db.Users.AddRange(
                CreateDoctorUser(passwordHasher, "dr.ahmad", "ahmad@hospitalbooking.local", "Ahmad Khalil", "أحمد خليل",
                    cardiologyId, "Cardiologist", "أمراض القلب", "LIC-1001"),
                CreateDoctorUser(passwordHasher, "dr.lina", "lina@hospitalbooking.local", "Lina Mansour", "لينا منصور",
                    orthoId, "Orthopedic Surgeon", "جراحة عظام", "LIC-1002"));

            await db.SaveChangesAsync();
            logger.LogInformation("Seeded demo doctors: dr.ahmad / Doctor@123 , dr.lina / Doctor@123");
        }

        if (!await db.Patients.AnyAsync())
        {
            db.Users.Add(new User
            {
                UserName = "sara",
                Email = "sara@hospitalbooking.local",
                PasswordHash = passwordHasher.HashPassword("Sara@123"),
                FullName = "Sara Ahmad",
                FullNameAr = "سارة أحمد",
                Phone = "0599000000",
                RoleId = (int)SystemRole.Patient,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Patient = new Patient
                {
                    NationalId = "123456789",
                    Gender = Gender.Female
                }
            });

            await db.SaveChangesAsync();
            logger.LogInformation("Seeded demo patient: sara / Sara@123");
        }
    }

    private static User CreateDoctorUser(
        IPasswordHasherService passwordHasher,
        string userName,
        string email,
        string fullName,
        string fullNameAr,
        int departmentId,
        string specialty,
        string specialtyAr,
        string license)
    {
        return new User
        {
            UserName = userName,
            Email = email,
            PasswordHash = passwordHasher.HashPassword("Doctor@123"),
            FullName = fullName,
            FullNameAr = fullNameAr,
            RoleId = (int)SystemRole.Doctor,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Doctor = new Doctor
            {
                DepartmentId = departmentId,
                Specialty = specialty,
                SpecialtyAr = specialtyAr,
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
                        DayOfWeek = DayOfWeek.Wednesday,
                        StartTime = new TimeOnly(10, 0),
                        EndTime = new TimeOnly(15, 0),
                        SlotDurationMinutes = 30,
                        IsActive = true
                    }
                ]
            }
        };
    }
}
