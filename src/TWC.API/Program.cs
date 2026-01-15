using TWC.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Host.ConfigureWolverine();

builder.AddInfrastructureServices();
builder.AddApplicationServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapServiceDefaults();
app.MapCarter();
app.UseExceptionHandler(_ => { });

await app.RunAsync();
