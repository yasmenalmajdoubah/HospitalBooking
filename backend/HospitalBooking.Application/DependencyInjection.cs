using Microsoft.Extensions.DependencyInjection;

namespace HospitalBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services / validators will be registered in later steps.
        return services;
    }
}
