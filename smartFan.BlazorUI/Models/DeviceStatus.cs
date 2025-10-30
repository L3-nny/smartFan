namespace smartFan.BlazorUI.Models;

/// <summary>
/// Represents the current status of a smart fan device
/// </summary>
public class DeviceStatus
{
    public int DeviceId { get; set; } = 1;
    public bool IsOnline { get; set; } = true;
    public double Temperature { get; set; }
    public string TemperatureUnit { get; set; } = "°C";
    public FanSpeed CurrentFanSpeed { get; set; }
    public FanMode Mode { get; set; }
    public int Rpm { get; set; }
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Fan speed levels matching the backend API
/// </summary>
public enum FanSpeed
{
    Off = 0,
    Low = 1,
    Medium = 2,
    High = 3
}

/// <summary>
/// Fan control mode
/// </summary>
public enum FanMode
{
    Auto,
    Manual
}

/// <summary>
/// Response from temperature API endpoint
/// </summary>
public class TemperatureResponse
{
    public double Temperature { get; set; }
    public string Unit { get; set; } = "°C";
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Response from fan speed API endpoint
/// </summary>
public class FanSpeedResponse
{
    public string FanSpeed { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Response from actuator adjust endpoint
/// </summary>
public class ActuatorResponse
{
    public double Temperature { get; set; }
    public string NewFanSpeed { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}