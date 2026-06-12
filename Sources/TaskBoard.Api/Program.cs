using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.JsonWebTokens;

using Serilog;

using TaskBoard.Api.Extensions;
using TaskBoard.Api.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

// Désactive le remappage automatique des claims
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.AddCustomConfiguration();
builder.Host.AddCustomSerilog();

// Services
builder.Services.AddCustomServices();
builder.Services.AddCustomSwagger();
builder.Services.AddControllers();
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.AddCustomDatabase(builder.Configuration);

// Auth
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);

// Health checks
builder.Services.AddCustomHealthChecks(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

// 🔥 ROUTING AVANT LES MIDDLEWARES
app.UseRouting();

// Middlewares custom
app.UseCustomMiddlewares();

// CORS
app.UseCustomCors();

// Auth
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health checks
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

// 🔥 ENDPOINTS APRÈS AUTH
app.MapControllers();

// Seed
await DatabaseSeeder.SeedAsync(app.Services);

app.Run();

public partial class Program { }
