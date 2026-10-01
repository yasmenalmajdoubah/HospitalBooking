using HospitalBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalBooking.Infrastructure.Persistence.Configurations;

public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
{
    public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
    {
        builder.ToTable("DoctorSchedules");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SlotDurationMinutes).HasDefaultValue(30);
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder.HasIndex(x => new { x.DoctorId, x.DayOfWeek, x.StartTime }).IsUnique();

        builder.HasOne(x => x.Doctor)
            .WithMany(x => x.Schedules)
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
