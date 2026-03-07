using smartFan.BlazorUI.Models;

namespace smartFan.BlazorUI.Services;

/// <summary>
/// Service interface for temperature-related operations
/// </summary>
public interface ITemperatureService
{
    /// <summary>
    /// Gets the current temperature reading
    /// </summary>
    Task<TemperatureResponse?> GetCurrentTemperatureAsync();
}

/// <summary>
/// Service interface for actuator/fan control operations
/// </summary>
public interface IActuatorService
{
    /// <summary>
    /// Gets the current fan speed
    /// </summary>
    Task<FanSpeedResponse?> GetCurrentFanSpeedAsync();
    
    /// <summary>
    /// Adjusts the fan speed based on temperature
    /// </summary>
    Task<ActuatorResponse?> AdjustFanSpeedAsync(double temperature);
    
    /// <summary>
    /// Manually sets the fan speed (override)
    /// </summary>
    Task<ActuatorResponse?> SetManualFanSpeedAsync(FanSpeed speed);
}

/// <summary>
/// Service interface for device status operations
/// </summary>
public interface IDeviceService
{
    /// <summary>
    /// Gets the complete device status including temperature and fan speed
    /// </summary>
    Task<DeviceStatus?> GetDeviceStatusAsync();
    
    /// <summary>
    /// Checks if the device is online and responsive
    /// </summary>
    Task<bool> IsDeviceOnlineAsync();
}