using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.JsonWebTokens;

using Serilog;

using TaskBoard.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Désactive le remappage automatique des claims (sub, role, email, etc.)
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.AddCustomConfiguration();
builder.Host.AddCustomSerilog();

builder.Services.AddCustomServices();
builder.Services.AddCustomSwagger();
builder.Services.AddControllers();
builder.Services.AddCustomDatabase(builder.Configuration);

// On passe l'environnement à l'extension
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);

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

public partial class Program { }
