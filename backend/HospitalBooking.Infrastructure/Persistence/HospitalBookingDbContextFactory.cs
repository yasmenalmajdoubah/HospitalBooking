using HospitalBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HospitalBooking.Infrastructure.Persistence;

/// <summary>
/// Used by `dotnet ef` at design time to create migrations.
/// </summary>
public class HospitalBookingDbContextFactory : IDesignTimeDbContextFactory<HospitalBookingDbContext>
{
    public HospitalBookingDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "..", "HospitalBooking.Api"));

        if (!Directory.Exists(basePath))
        {
            basePath = Path.GetFullPath(
                Path.Combine(Directory.GetCurrentDirectory(), "HospitalBooking.Api"));
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("HospitalBookingDb")
            ?? throw new InvalidOperationException("Connection string 'HospitalBookingDb' was not found.");

        var provider = configuration.GetValue<string>("Database:Provider") ?? "Sqlite";
        var optionsBuilder = new DbContextOptionsBuilder<HospitalBookingDbContext>();

        if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        else
        {
            optionsBuilder.UseSqlite(connectionString);
        }

        return new HospitalBookingDbContext(optionsBuilder.Options);
    }
}
