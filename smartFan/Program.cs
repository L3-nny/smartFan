using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using smartFan.Data;
using smartFan.Repositories.Interfaces;
using smartFan.Repositories.EfCore;
using smartFan.Services;
using smartFan.Services.Interfaces;
using smartFan.Middleware;
using smartFan.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add CORS for Blazor WebAssembly app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "https://localhost:7291", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add .NET's built-in Memory Cache
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Smart Fan API", 
        Version = "v1",
        Description = "A smart temperature monitoring and fan control system"
    });
    
    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Add DbContext
builder.Services.AddDbContext<smartFanContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SmartFanDb")));

// Database write guard: set via configuration or environment variable
// Default to true (disable writes) to avoid creating/migrating DB during local dev
var disableDbWrites = builder.Configuration.GetValue<bool>("DisableDatabaseWrites", true);
builder.Services.AddSingleton(new DatabaseWriteGuard(disableDbWrites));

// Register Repository interfaces with EF Core implementations
// Note: DbContext remains scoped, but we'll handle this in repository implementations
builder.Services.AddScoped<IDeviceConfigRepository, DeviceConfigRepository>();
builder.Services.AddScoped<ITemperatureLogRepository, TemperatureLogRepository>();
builder.Services.AddScoped<IErrorLogRepository, ErrorLogRepository>();

// Register Services - keep these scoped for proper EF Core usage
builder.Services.AddScoped<DeviceConfigService>();
builder.Services.AddScoped<TemperatureLogService>();
builder.Services.AddScoped<ErrorLogService>();
builder.Services.AddScoped<SystemService>();

// Register existing services with their interfaces
// Only ActuatorService needs to be singleton to maintain fan state across requests
builder.Services.AddScoped<ISensorService, SensorSimulator>();
builder.Services.AddSingleton<IActuatorService, ActuatorService>(); // Shared fan state for dashboard
builder.Services.AddScoped<ILoggerService, LoggerService>();
builder.Services.AddScoped<IRandomProvider, RandomProvider>();

//Register the FanSettings from configuration
builder.Services.Configure<FanSettingsModel>(builder.Configuration.GetSection("FanSettings"));

// Register Background Services
builder.Services.AddHostedService<BackgroundMonitorService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors("AllowBlazorApp");
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

// Attempt safe startup migration if writes are enabled. If writes are disabled we skip migrations.
using (var startupScope = app.Services.CreateScope())
{
    var guard = startupScope.ServiceProvider.GetRequiredService<DatabaseWriteGuard>();
    if (!guard.DisableWrites)
    {
        try
        {
            var ctx = startupScope.ServiceProvider.GetRequiredService<smartFanContext>();
            ctx.Database.Migrate();
            Console.WriteLine("[Program] Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Program] Warning: database migration failed (non-fatal): {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine("[Program] Database writes are disabled; skipping migrations.");
    }
}

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"[Program] Host terminated with exception: {ex.Message}");
    // Swallow the exception to avoid crashing the process on startup/runtime DB errors.
}