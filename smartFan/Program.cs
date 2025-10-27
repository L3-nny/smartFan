using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using smartFan.Data;
using smartFan.Repositories.Interfaces;
using smartFan.Repositories.EfCore;
using smartFan.Services;
using smartFan.Services.Interfaces;
using smartFan.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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

// Register Repository interfaces with EF Core implementations
builder.Services.AddScoped<IDeviceConfigRepository, DeviceConfigRepository>();
builder.Services.AddScoped<ITemperatureLogRepository, TemperatureLogRepository>();
builder.Services.AddScoped<IErrorLogRepository, ErrorLogRepository>();

// Register Services
builder.Services.AddScoped<DeviceConfigService>();
builder.Services.AddScoped<TemperatureLogService>();
builder.Services.AddScoped<ErrorLogService>();
builder.Services.AddScoped<SystemService>();

// Register existing services with their interfaces
builder.Services.AddScoped<ISensorService, SensorSimulator>();
builder.Services.AddSingleton<IActuatorService, ActuatorService>(); // Shared fan state for dashboard
builder.Services.AddScoped<ILoggerService, LoggerService>();

// Register IRandomProvider with a concrete implementation
builder.Services.AddScoped<IRandomProvider, RandomProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

app.Run();