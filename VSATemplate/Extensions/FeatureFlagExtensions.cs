using Microsoft.FeatureManagement;

namespace VSATemplate.Extensions;

public static class FeatureFlagExtensions
{
    public static IServiceCollection AddFeatureFlagServices(this IServiceCollection services)
    {
        services.AddFeatureManagement();
        
        return services;
    }
}

