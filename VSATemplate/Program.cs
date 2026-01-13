var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;


builder.Host.UseWolverine(WolverineConfiguration.Configure);
builder.Host.UseSerilog(SerilogConfiguration.Configure);

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
app.UseHealthChecks();
app.MapCarter();
app.UseExceptionHandler(_ => { });

await app.RunAsync();