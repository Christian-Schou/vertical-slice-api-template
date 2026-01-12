namespace VSATemplate.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(context =>
            context.UseNpgsql(configuration.GetConnectionString("Database")));

        return services;
    }
}