using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using smartFan.BlazorUI.Models;

namespace smartFan.BlazorUI.Services;

/// <summary>
/// Implementation of actuator service using HttpClient
/// </summary>
public class ActuatorService : IActuatorService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ActuatorService> _logger;

    public ActuatorService(HttpClient httpClient, ILogger<ActuatorService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<FanSpeedResponse?> GetCurrentFanSpeedAsync()
    {
        try
        {
            _logger.LogInformation("Fetching current fan speed from API");
            var response = await _httpClient.GetFromJsonAsync<FanSpeedResponse>("api/Actuator/speed");
            _logger.LogInformation("Successfully retrieved fan speed: {FanSpeed}", response?.FanSpeed);
            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching fan speed");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching fan speed");
            return null;
        }
    }

    public async Task<ActuatorResponse?> AdjustFanSpeedAsync(double temperature)
    {
        try
        {
            _logger.LogInformation("Adjusting fan speed for temperature: {Temperature}°C", temperature);
            
            var response = await _httpClient.PostAsJsonAsync("api/Actuator/adjust", temperature);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ActuatorResponse>();
                _logger.LogInformation("Successfully adjusted fan speed to: {FanSpeed}", result?.NewFanSpeed);
                return result;
            }
            else
            {
                _logger.LogWarning("API returned non-success status code: {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while adjusting fan speed");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while adjusting fan speed");
            return null;
        }
    }

    public async Task<ActuatorResponse?> SetManualFanSpeedAsync(FanSpeed speed)
    {
        try
        {
            _logger.LogInformation("Setting manual fan speed: {FanSpeed}", speed);
            
            // Convert fan speed to temperature range for API compatibility
            double temperature = speed switch
            {
                FanSpeed.Off => 20.0,      // Below 25°C threshold
                FanSpeed.Low => 27.0,      // 25-30°C range
                FanSpeed.Medium => 32.0,   // 30-34°C range  
                FanSpeed.High => 36.0,     // Above 34°C threshold
                _ => 20.0
            };

            return await AdjustFanSpeedAsync(temperature);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while setting manual fan speed");
            return null;
        }
    }
}