using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core DbContext + repositories will be registered in Step 04.
        // Connection string key: ConnectionStrings:HospitalBookingDb
        _ = configuration;
        return services;
    }
}
