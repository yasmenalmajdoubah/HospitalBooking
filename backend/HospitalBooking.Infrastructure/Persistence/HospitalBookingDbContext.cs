using HospitalBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalBooking.Infrastructure.Persistence;

public class HospitalBookingDbContext : DbContext
{
    public HospitalBookingDbContext(DbContextOptions<HospitalBookingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
    public DbSet<AppointmentStatus> AppointmentStatuses => Set<AppointmentStatus>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HospitalBookingDbContext).Assembly);
        DbSeed.SeedLookupData(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
