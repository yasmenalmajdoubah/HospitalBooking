using HospitalBooking.Application.Interfaces;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Enums;
using HospitalBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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
        var db = scope.ServiceProvider.GetRequiredService<HospitalBookingDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

        try
        {
            if (applyMigrations)
            {
                await db.Database.MigrateAsync();
                logger.LogInformation("Applied EF migrations to HospitalBookingDb (if any were pending).");
            }

            if (!await db.Database.CanConnectAsync())
            {
                logger.LogWarning("Cannot connect to HospitalBookingDb. Skipping admin seed.");
                return;
            }

            var adminExists = await db.Users.AnyAsync(x => x.UserName == DefaultAdminUserName);
            if (adminExists)
            {
                return;
            }

            var admin = new User
            {
                UserName = DefaultAdminUserName,
                Email = "admin@hospitalbooking.local",
                PasswordHash = passwordHasher.HashPassword(DefaultAdminPassword),
                FullName = "System Administrator",
                FullNameAr = "مدير النظام",
                RoleId = (int)SystemRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(admin);
            await db.SaveChangesAsync();
            logger.LogInformation(
                "Seeded default admin user '{UserName}'. Change the password after first login.",
                DefaultAdminUserName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database initialize/seed skipped. Install SQL Server LocalDB and run: dotnet ef database update");
        }
    }
}
