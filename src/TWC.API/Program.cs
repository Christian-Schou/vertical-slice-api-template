using JasperFx;
using TWC.ServiceDefaults;
using Marten;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddMarten(opts =>
{
    var connectionString = builder.Configuration.GetConnectionString("Database");
    opts.Connection(connectionString!);

    // Track the number of events being appended to the system
    opts.OpenTelemetry.TrackEventCounters();
})
.IntegrateWithWolverine();

var assembly = typeof(Program).Assembly;


builder.Host.UseWolverine(opts =>
{
    opts.AutoBuildMessageStorageOnStartup = AutoCreate.CreateOrUpdate;
    opts.Policies.AutoApplyTransactions();
    
    opts.UseFluentValidation();
});

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices(assembly);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.MapServiceDefaults();
app.MapCarter();
app.UseExceptionHandler(_ => { });

await app.RunAsync();
