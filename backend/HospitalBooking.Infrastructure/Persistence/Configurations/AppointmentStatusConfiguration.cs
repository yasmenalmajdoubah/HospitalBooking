using HospitalBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalBooking.Infrastructure.Persistence.Configurations;

public class AppointmentStatusConfiguration : IEntityTypeConfiguration<AppointmentStatus>
{
    public void Configure(EntityTypeBuilder<AppointmentStatus> builder)
    {
        builder.ToTable("AppointmentStatuses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
