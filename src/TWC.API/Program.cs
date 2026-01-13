using TWC.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var assembly = typeof(Program).Assembly;


builder.Host.UseWolverine(WolverineConfiguration.Configure);

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
