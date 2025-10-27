using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using smartFan.Services.Interfaces;
using System;

namespace smartFan.Controllers
{
    /// <summary>
    /// Provides sensor data access and management functionality.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly ISensorService _sensorService;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes the <see cref="SensorController"/>.
        /// </summary>
        public SensorController(ISensorService sensorService, ILoggerService logger, IMemoryCache cache)
        {
            _sensorService = sensorService;
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// Gets the next temperature reading from the sensor.
        /// </summary>
        /// <returns>Temperature reading in Celsius.</returns>
        [HttpGet("temperature")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetTemperature()
        {
            try
            {
                const string cacheKey = "current_temperature";
                
                // Use IMemoryCache.GetOrCreate - this is the recommended pattern
                var result = _cache.GetOrCreate(cacheKey, entry =>
                {
                    // Set cache expiration
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
                    
                    // Get fresh reading - this only executes if not in cache
                    double temperature = _sensorService.GetNextTemperature();
                    _logger.LogInfo($"Fresh sensor temperature reading: {temperature:F2}°C");

                    return new
                    {
                        Temperature = temperature,
                        Unit = "°C",
                        SensorType = "Simulated",
                        Timestamp = DateTime.UtcNow
                    };
                });

                _logger.LogInfo("Temperature reading retrieved (cached or fresh)");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reading sensor temperature", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to read sensor data.");
            }
        }

        /// <summary>
        /// Gets multiple temperature readings for trending analysis.
        /// </summary>
        /// <param name="count">Number of readings to collect (1-100).</param>
        /// <returns>Array of temperature readings.</returns>
        [HttpGet("temperature/batch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetTemperatureBatch([FromQuery] int count = 5)
        {
            try
            {
                if (count < 1 || count > 100)
                {
                    return BadRequest("Count must be between 1 and 100");
                }

                var readings = new List<object>();
                for (int i = 0; i < count; i++)
                {
                    readings.Add(new
                    {
                        Temperature = _sensorService.GetNextTemperature(),
                        ReadingNumber = i + 1,
                        Timestamp = DateTime.UtcNow
                    });
                    
                    // Small delay between readings for more realistic data
                    if (i < count - 1)
                    {
                        Thread.Sleep(100);
                    }
                }

                _logger.LogInfo($"Collected {count} sensor readings");

                return Ok(new
                {
                    Readings = readings,
                    Count = count,
                    Unit = "°C",
                    CollectionTime = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error collecting {count} sensor readings", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to collect sensor readings.");
            }
        }

        /// <summary>
        /// Gets sensor status and information.
        /// </summary>
        /// <returns>Sensor status details.</returns>
        [HttpGet("status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetSensorStatus()
        {
            try
            {
                // Since we're using a simulated sensor, provide mock status info
                var status = new
                {
                    SensorType = "Temperature Sensor Simulator",
                    Status = "Online",
                    LastReading = _sensorService.GetNextTemperature(),
                    Unit = "°C",
                    Accuracy = "±0.5°C",
                    Range = new { Min = -40, Max = 125 },
                    Timestamp = DateTime.UtcNow
                };

                _logger.LogInfo("Sensor status requested");

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting sensor status", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get sensor status.");
            }
        }
    }
}