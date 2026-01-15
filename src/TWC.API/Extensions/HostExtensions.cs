using JasperFx;
namespace TWC.API.Extensions;

public static class HostExtensions
{
    public static IHostBuilder ConfigureWolverine(this IHostBuilder host)
    {
        return host.UseWolverine(opts =>
        {
            opts.AutoBuildMessageStorageOnStartup = AutoCreate.CreateOrUpdate;
            opts.Policies.AutoApplyTransactions();
            
            opts.UseFluentValidation();
        });
    }
}

