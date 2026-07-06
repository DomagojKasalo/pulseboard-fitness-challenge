using FitnessChallenge.Api.Application.Services;
using FitnessChallenge.Api.Application.Validation;
using FitnessChallenge.Api.Domain.Scoring;
using FitnessChallenge.Api.Infrastructure;
using FitnessChallenge.Api.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicy = "frontend";
var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=fitness.db";

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<FitnessDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton(ScoringStrategyFactory.CreateService());

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<IngestActivityRequestValidator>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options => options.AddPolicy(CorsPolicy, policy =>
    policy.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FitnessDbContext>();
    await db.Database.MigrateAsync();

    if (!app.Environment.IsEnvironment("Testing"))
    {
        var scoring = scope.ServiceProvider.GetRequiredService<IScoringService>();
        var clock = scope.ServiceProvider.GetRequiredService<TimeProvider>();
        await DbSeeder.SeedAsync(db, scoring, clock);
    }
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors(CorsPolicy);
app.MapControllers();

app.Run();

public partial class Program;
