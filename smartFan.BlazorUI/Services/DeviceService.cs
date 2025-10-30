using smartFan.BlazorUI.Models;

namespace smartFan.BlazorUI.Services;

/// <summary>
/// Combined device service that aggregates temperature and actuator data
/// </summary>
public class DeviceService : IDeviceService
{
    private readonly ITemperatureService _temperatureService;
    private readonly IActuatorService _actuatorService;
    private readonly ILogger<DeviceService> _logger;

    public DeviceService(
        ITemperatureService temperatureService, 
        IActuatorService actuatorService,
        ILogger<DeviceService> logger)
    {
        _temperatureService = temperatureService;
        _actuatorService = actuatorService;
        _logger = logger;
    }

    public async Task<DeviceStatus?> GetDeviceStatusAsync()
    {
        try
        {
            _logger.LogInformation("Fetching complete device status");

            // Fetch temperature and fan speed concurrently
            var temperatureTask = _temperatureService.GetCurrentTemperatureAsync();
            var fanSpeedTask = _actuatorService.GetCurrentFanSpeedAsync();

            await Task.WhenAll(temperatureTask, fanSpeedTask);

            var temperature = temperatureTask.Result;
            var fanSpeed = fanSpeedTask.Result;

            if (temperature == null || fanSpeed == null)
            {
                _logger.LogWarning("Failed to retrieve complete device status - one or more API calls failed");
                return new DeviceStatus
                {
                    IsOnline = false,
                    Temperature = 0,
                    CurrentFanSpeed = FanSpeed.Off,
                    Mode = FanMode.Auto,
                    LastUpdated = DateTime.Now
                };
            }

            // Parse fan speed string to enum
            var fanSpeedEnum = ParseFanSpeed(fanSpeed.FanSpeed);
            
            // Calculate estimated RPM based on fan speed
            var rpm = CalculateRpm(fanSpeedEnum);

            var deviceStatus = new DeviceStatus
            {
                IsOnline = true,
                Temperature = temperature.Temperature,
                TemperatureUnit = temperature.Unit,
                CurrentFanSpeed = fanSpeedEnum,
                Mode = FanMode.Auto, // TODO: Determine actual mode from API
                Rpm = rpm,
                LastUpdated = DateTime.Now
            };

            _logger.LogInformation("Successfully retrieved device status: {Temperature}°C, {FanSpeed}", 
                deviceStatus.Temperature, deviceStatus.CurrentFanSpeed);

            return deviceStatus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching device status");
            return new DeviceStatus
            {
                IsOnline = false,
                Temperature = 0,
                CurrentFanSpeed = FanSpeed.Off,
                Mode = FanMode.Auto,
                LastUpdated = DateTime.Now
            };
        }
    }

    public async Task<bool> IsDeviceOnlineAsync()
    {
        try
        {
            var status = await GetDeviceStatusAsync();
            return status?.IsOnline ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking device online status");
            return false;
        }
    }

    private FanSpeed ParseFanSpeed(string fanSpeedString)
    {
        return fanSpeedString.ToUpperInvariant() switch
        {
            "OFF" => FanSpeed.Off,
            "LOW" => FanSpeed.Low,
            "MEDIUM" => FanSpeed.Medium,
            "HIGH" => FanSpeed.High,
            _ => FanSpeed.Off
        };
    }

    private int CalculateRpm(FanSpeed fanSpeed)
    {
        return fanSpeed switch
        {
            FanSpeed.Off => 0,
            FanSpeed.Low => 300,
            FanSpeed.Medium => 600,
            FanSpeed.High => 900,
            _ => 0
        };
    }
}