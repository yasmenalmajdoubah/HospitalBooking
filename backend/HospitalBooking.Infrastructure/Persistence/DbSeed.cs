using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HospitalBooking.Infrastructure.Persistence;

public static class DbSeed
{
    public static void SeedLookupData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = (int)SystemRole.Admin, Name = "Admin", NameAr = "مدير النظام" },
            new Role { Id = (int)SystemRole.Doctor, Name = "Doctor", NameAr = "طبيب" },
            new Role { Id = (int)SystemRole.Receptionist, Name = "Receptionist", NameAr = "موظف استقبال" },
            new Role { Id = (int)SystemRole.Patient, Name = "Patient", NameAr = "مريض" }
        );

        modelBuilder.Entity<AppointmentStatus>().HasData(
            new AppointmentStatus { Id = (int)AppointmentStatusCode.Pending, Name = "Pending", NameAr = "قيد الانتظار" },
            new AppointmentStatus { Id = (int)AppointmentStatusCode.Confirmed, Name = "Confirmed", NameAr = "مؤكد" },
            new AppointmentStatus { Id = (int)AppointmentStatusCode.Cancelled, Name = "Cancelled", NameAr = "ملغي" },
            new AppointmentStatus { Id = (int)AppointmentStatusCode.Completed, Name = "Completed", NameAr = "مكتمل" }
        );
    }
}
