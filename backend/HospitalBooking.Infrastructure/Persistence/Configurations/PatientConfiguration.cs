using HospitalBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalBooking.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.NationalId).HasMaxLength(50);
        builder.Property(x => x.Gender)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("[UserId] IS NOT NULL");

        builder.HasIndex(x => x.NationalId)
            .IsUnique()
            .HasFilter("[NationalId] IS NOT NULL");
    }
}
