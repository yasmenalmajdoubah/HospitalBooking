using HospitalBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HospitalBookingDb")
            ?? throw new InvalidOperationException(
                "Connection string 'HospitalBookingDb' is missing. Check appsettings.json.");

        services.AddDbContext<HospitalBookingDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
