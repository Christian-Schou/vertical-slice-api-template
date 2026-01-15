using System.Reflection;
using JasperFx;
using Marten;
using Wolverine.Marten;

namespace TWC.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddInfrastructureServices(this WebApplicationBuilder builder)
    {
        builder.Host.UseWolverine(opts =>
        {
            opts.AutoBuildMessageStorageOnStartup = builder.Environment.IsProduction() 
                ? AutoCreate.None 
                : AutoCreate.CreateOrUpdate;
                
            opts.Policies.AutoApplyTransactions();
            
            opts.UseFluentValidation();
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.AddMarten(opts =>
        {
            var connectionString = builder.Configuration.GetConnectionString("Database");
            opts.Connection(connectionString!);

            // Track the number of events being appended to the system
            opts.OpenTelemetry.TrackEventCounters();
        })
        .IntegrateWithWolverine();

        builder.Services.AddIdentityServices(builder.Configuration);

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Services.AddCarter();

        return builder;
    }
}