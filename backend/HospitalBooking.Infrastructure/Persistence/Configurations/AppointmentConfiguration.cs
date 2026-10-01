using HospitalBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalBooking.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Notes).HasMaxLength(1000);
        // CreatedAt is set in application code (UTC).

        builder.HasIndex(x => new { x.DoctorId, x.AppointmentDate, x.StartTime }).IsUnique();
        builder.HasIndex(x => new { x.PatientId, x.AppointmentDate });

        builder.HasOne(x => x.Patient)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Doctor)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedAppointments)
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
