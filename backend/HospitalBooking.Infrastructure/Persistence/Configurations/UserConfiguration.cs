using HospitalBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalBooking.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.FullNameAr).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        // CreatedAt is set in application code (UTC).

        builder.HasIndex(x => x.UserName).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Doctor)
            .WithOne(x => x.User)
            .HasForeignKey<Doctor>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Patient)
            .WithOne(x => x.User)
            .HasForeignKey<Patient>(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
