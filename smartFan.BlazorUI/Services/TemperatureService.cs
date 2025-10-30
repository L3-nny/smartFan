using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using smartFan.BlazorUI.Models;

namespace smartFan.BlazorUI.Services;

/// <summary>
/// Implementation of temperature service using HttpClient
/// </summary>
public class TemperatureService : ITemperatureService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TemperatureService> _logger;

    public TemperatureService(HttpClient httpClient, ILogger<TemperatureService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TemperatureResponse?> GetCurrentTemperatureAsync()
    {
        try
        {
            _logger.LogInformation("Fetching current temperature from API");
            var response = await _httpClient.GetFromJsonAsync<TemperatureResponse>("api/Temperature/current");
            _logger.LogInformation("Successfully retrieved temperature: {Temperature}°C", response?.Temperature);
            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching temperature");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching temperature");
            return null;
        }
    }
}