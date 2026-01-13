namespace TWC.API.Extensions;

public static class HealthChecksExtensions
{
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();
        return services;
    }
}