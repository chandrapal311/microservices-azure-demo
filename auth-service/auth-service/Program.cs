using auth_service.Data;
using auth_service.Services;
using auth_service.Telemetry;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add OpenTelemetry distributed tracing (feature flag controlled)
builder.Services.AddCustomTelemetry(builder.Configuration);

builder.Services.AddScoped<TokenService>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
