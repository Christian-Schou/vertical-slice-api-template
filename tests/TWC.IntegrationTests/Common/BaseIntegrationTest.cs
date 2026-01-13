using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using TWC.API.Database;

namespace TWC.IntegrationTests.Common;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private static Respawner? _respawner;
    private readonly IServiceScope _scope;
    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        DbContext.Database.EnsureCreated();
    }

    public async Task InitializeAsync()
    {
        var connection = DbContext.Database.GetDbConnection();
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