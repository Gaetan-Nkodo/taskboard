using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using TaskBoard.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
builder.Host.AddCustomSerilog();

builder.Services.AddCustomServices();
builder.Services.AddCustomSwagger();
builder.Services.AddControllers();
builder.Services.AddCustomDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomHealthChecks(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCustomMiddlewares();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapControllers();

app.Run();
