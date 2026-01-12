var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseWolverine(options => { options.UseFluentValidation(); });

builder.Services
    .AddDatabaseServices(builder.Configuration)
    .AddValidatorsFromAssembly(assembly)
    .AddCarter()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddHealthCheckServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHealthChecks();
app.MapCarter();
app.UseExceptionHandler(_ => { });

await app.RunAsync();