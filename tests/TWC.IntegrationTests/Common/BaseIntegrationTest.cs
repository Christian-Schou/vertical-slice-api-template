using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;

namespace TWC.IntegrationTests.Common;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private static Respawner? _respawner;
    private readonly IServiceScope _scope;
    protected readonly IDocumentSession Session;
    protected readonly IDocumentStore Store;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Store = _scope.ServiceProvider.GetRequiredService<IDocumentStore>();
        Session = _scope.ServiceProvider.GetRequiredService<IDocumentSession>();
    }

    public async Task InitializeAsync()
    {
        var configuration = _scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("Database");
        
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        if (_respawner == null)
            _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" }
            });

        await _respawner.ResetAsync(connection);
    }

    public Task DisposeAsync()
    {
        _scope.Dispose(); // Good practice to dispose scope
        return Task.CompletedTask;
    }
}