using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.JsonWebTokens;

using Serilog;

using TaskBoard.Api.Extensions;
using TaskBoard.Api.Infrastructure.Database;
using TaskBoard.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// Désactive le remappage automatique des claims
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Chargement de la configuration (inclut appsettings.Development.json)
builder.AddCustomConfiguration();

// Serilog
builder.Host.AddCustomSerilog();

// 🔥 Charger EmailSettings AVANT AddCustomServices
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

// Services
builder.Services.AddCustomServices();
builder.Services.AddCustomSwagger();
builder.Services.AddControllers();
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.AddCustomDatabase(builder.Configuration);

// Auth
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);

// 🔥 Choix du sender selon l'environnement
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
}
else
{
    builder.Services.AddScoped<IEmailSender, MailjetEmailSender>();
}

// Health checks
builder.Services.AddCustomHealthChecks(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

// Routing
app.UseRouting();

// Middlewares custom
app.UseCustomMiddlewares();

// CORS
app.UseCustomCors();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Swagger uniquement en dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health checks
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

// Endpoints
app.MapControllers();

// Seed
await DatabaseSeeder.SeedAsync(app.Services);

// 🔥 Endpoint debug email (uniquement en dev)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/debug/email", () =>
    {
        const string path = "sandbox-email-last.html";

        if (!File.Exists(path))
            return Results.NotFound("Aucun email n'a été envoyé pour le moment.");

        var html = File.ReadAllText(path);
        return Results.Content(html, "text/html");
    });
}

app.Run();

public partial class Program { }
