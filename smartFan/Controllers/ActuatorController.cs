using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using smartFan.Services.Interfaces;
using System;

namespace smartFan.Controllers
{
    /// <summary>
    /// Provides fan control and actuator management functionality.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ActuatorController : ControllerBase
    {
        private readonly IActuatorService _actuatorService;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes the <see cref="ActuatorController"/>.
        /// </summary>
        public ActuatorController(IActuatorService actuatorService, ILoggerService logger, IMemoryCache cache)
        {
            _actuatorService = actuatorService;
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// Gets the current fan speed setting.
        /// </summary>
        /// <returns>The current fan speed level.</returns>
        [HttpGet("speed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCurrentFanSpeed()
        {
            try
            {
                const string cacheKey = "current_fan_speed";
                
                // Use IMemoryCache.GetOrCreate - simpler and more efficient
                var result = _cache.GetOrCreate(cacheKey, entry =>
                {
                    // Set cache expiration
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                    
                    var currentSpeed = _actuatorService.CurrentSpeed;
                    _logger.LogInfo($"Fresh fan speed reading: {currentSpeed}");

                    return new
                    {
                        FanSpeed = currentSpeed.ToString(),
                        Timestamp = DateTime.UtcNow
                    };
                });

                _logger.LogInfo("Fan speed retrieved (cached or fresh)");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting current fan speed", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get fan speed.");
            }
        }

        /// <summary>
        /// Adjusts the fan speed based on the provided temperature.
        /// </summary>
        /// <param name="temperature">The temperature reading to base fan control on.</param>
        /// <returns>The new fan speed setting.</returns>
        [HttpPost("adjust")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AdjustFanSpeed([FromBody] double temperature)
        {
            try
            {
                if (temperature < 20 || temperature > 45)
                {
                    return BadRequest("Temperature must be between 20°C and 45°C");
                }

                _actuatorService.Update(temperature);
                var newSpeed = _actuatorService.CurrentSpeed;
                
                // Invalidate the fan speed cache since it changed
                _cache.Remove("current_fan_speed");
                
                _logger.LogInfo($"Fan speed adjusted for temperature {temperature:F1}°C: {newSpeed}");

                return Ok(new
                {
                    Temperature = temperature,
                    NewFanSpeed = newSpeed.ToString(),
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adjusting fan speed for temperature {temperature}°C", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to adjust fan speed.");
            }
        }

        /// <summary>
        /// Gets the available fan speed levels.
        /// </summary>
        /// <returns>List of available fan speed options.</returns>
        [HttpGet("speeds")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAvailableSpeeds()
        {
            try
            {
                var speeds = Enum.GetValues<smartFan.Services.Interfaces.FanSpeed>()
                    .Select(speed => new
                    {
                        Name = speed.ToString(),
                        Level = (int)speed
                    })
                    .ToArray();

                return Ok(new
                {
                    AvailableSpeeds = speeds,
                    CurrentSpeed = _actuatorService.CurrentSpeed.ToString(),
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting available fan speeds", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get available speeds.");
            }
        }
    }
}