using Microsoft.Extensions.DependencyInjection;

namespace HospitalBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Application-layer services can be registered here later.
        return services;
    }
}
