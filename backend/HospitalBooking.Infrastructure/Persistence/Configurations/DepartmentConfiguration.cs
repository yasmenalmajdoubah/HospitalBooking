using HospitalBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalBooking.Infrastructure.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
