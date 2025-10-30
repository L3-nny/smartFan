using smartFan.Services.Interfaces;

namespace smartFan.Services
{
    /// <summary>
    /// Background service that automatically monitors temperature and adjusts fan speed.
    /// Runs continuously while the application is running.
    /// </summary>
    public class BackgroundMonitorService : BackgroundService
    {
        private readonly IActuatorService _actuatorService; // Singleton, safe to inject
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _monitoringInterval;

        public BackgroundMonitorService(
            IActuatorService actuatorService, // Only inject singleton services directly
            IServiceProvider serviceProvider)
        {
            _actuatorService = actuatorService;
            _serviceProvider = serviceProvider;
            _monitoringInterval = TimeSpan.FromSeconds(5); // Check every 5 seconds
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("[BackgroundMonitor] Background Monitor Service started - automatic fan control active");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await MonitorAndAdjustAsync(stoppingToken);
                    await Task.Delay(_monitoringInterval, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[BackgroundMonitor] Background Monitor Service stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BackgroundMonitor] Critical error in Background Monitor Service: {ex.Message}");
                throw; // Re-throw to indicate service failure
            }
        }

        private async Task MonitorAndAdjustAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Create a scope to access scoped services
                using var scope = _serviceProvider.CreateScope();
                var sensorService = scope.ServiceProvider.GetRequiredService<ISensorService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILoggerService>();
                
                // Get current temperature reading
                double currentTemperature = sensorService.GetNextTemperature();
                
                // Store previous fan speed for comparison
                var previousSpeed = _actuatorService.CurrentSpeed;
                
                // Update actuator based on temperature
                _actuatorService.Update(currentTemperature);
                
                var newSpeed = _actuatorService.CurrentSpeed;

                // Log system state
                logger.LogSystemState(currentTemperature, newSpeed, deviceId: 1);

                // Log speed changes
                if (previousSpeed != newSpeed)
                {
                    Console.WriteLine($"[BackgroundMonitor] Fan speed changed: {previousSpeed} → {newSpeed} (Temp: {currentTemperature:F1}°C)");
                    logger.LogInfo($"Fan speed changed: {previousSpeed} → {newSpeed} (Temp: {currentTemperature:F1}°C)");
                }

                // Optional: Log temperature data to database
                await LogTemperatureDataAsync(currentTemperature, newSpeed, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BackgroundMonitor] Error during temperature monitoring cycle: {ex.Message}");
                // Continue monitoring even if one cycle fails
            }
        }

        private async Task LogTemperatureDataAsync(double temperature, FanSpeed fanSpeed, CancellationToken stoppingToken)
        {
            try
            {
                // Use scoped service for database operations
                using var scope = _serviceProvider.CreateScope();
                var tempLogService = scope.ServiceProvider.GetService<TemperatureLogService>();
                
                if (tempLogService != null)
                {
                    var temperatureLog = new TemperatureLog
                    {
                        DeviceConfigId = 1, // Default device
                        Temperature = temperature,
                        FanSpeed = fanSpeed.ToString(),
                        Timestamp = DateTime.UtcNow,
                        Source = "BackgroundMonitor",
                        LogLevel = "Info",
                        Message = $"Automatic monitoring: {temperature:F1}°C → {fanSpeed}"
                    };

                    await tempLogService.CreateLogAsync(temperatureLog);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BackgroundMonitor] Failed to log temperature data to database: {ex.Message}");
                // Don't stop monitoring if database logging fails
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[BackgroundMonitor] Starting Background Monitor Service...");
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[BackgroundMonitor] Stopping Background Monitor Service...");
            await base.StopAsync(cancellationToken);
        }
    }
}
