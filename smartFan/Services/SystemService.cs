using smartFan.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace smartFan.Services
{
    public class SystemService
    {
        private readonly ILoggerService _logger;
        private readonly ISensorService _sensorService;
        private readonly IActuatorService _actuatorService;

        public SystemService(ILoggerService logger, ISensorService sensorService, IActuatorService actuatorService)
        {
            _logger = logger;
            _sensorService = sensorService;
            _actuatorService = actuatorService;
        }

        public Task<object> GetSystemStatusAsync()
        {
            try
            {
                _logger.LogInfo("Retrieving system status");
                
                var currentTemp = _sensorService.GetNextTemperature();
                var fanSpeed = _actuatorService.CurrentSpeed;

                var status = new
                {
                    Temperature = currentTemp,
                    FanSpeed = fanSpeed.ToString(),
                    Status = "Running",
                    LastUpdated = DateTime.UtcNow
                };

                return Task.FromResult<object>(status);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving system status", ex);
                throw;
            }
        }

        public async Task<bool> StartSystemAsync()
        {
            try
            {
                _logger.LogInfo("Starting system");
                // Add system startup logic here
                await Task.Delay(100); // Simulate startup time
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error starting system", ex);
                throw;
            }
        }

        public async Task<bool> StopSystemAsync()
        {
            try
            {
                _logger.LogInfo("Stopping system");
                // Add system shutdown logic here
                await Task.Delay(100); // Simulate shutdown time
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error stopping system", ex);
                throw;
            }
        }

        public void LogSystemState(double temperature, int deviceId)
        {
            try
            {
                var fanSpeed = _actuatorService.CurrentSpeed;
                _logger.LogSystemState(temperature, fanSpeed, deviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error logging system state", ex);
                throw;
            }
        }
    }
}
