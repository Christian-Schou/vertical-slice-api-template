namespace VSATemplate.Configurations;

public static class SerilogConfiguration
{
    public static void Configure(HostBuilderContext context, LoggerConfiguration configuration)
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console();
    }
}

