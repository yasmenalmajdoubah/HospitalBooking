using HospitalBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalBooking.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Specialty).HasMaxLength(150).IsRequired();
        builder.Property(x => x.SpecialtyAr).HasMaxLength(150).IsRequired();
        builder.Property(x => x.LicenseNumber).HasMaxLength(50).IsRequired();

        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasIndex(x => x.LicenseNumber).IsUnique();

        builder.HasOne(x => x.Department)
            .WithMany(x => x.Doctors)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
