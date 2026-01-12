namespace VSATemplate.Configurations;

public static class WolverineConfiguration
{
    public static void Configure(WolverineOptions options)
    {
        options.UseFluentValidation();
    }
}

