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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCurrentFanSpeed()
        {
            try
            {
                const string cacheKey = "current_fan_speed";
                
                var result = _cache.GetOrCreate(cacheKey, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                    
                    var currentSpeed = _actuatorService.CurrentSpeed;
                    _logger.LogInfo($"New fan speed reading: {currentSpeed}");

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
                if (temperature < 10 || temperature > 53)
                {
                    return BadRequest("Temperature must be between 10°C and 53°C");
                }

                _actuatorService.Update(temperature);
                var newSpeed = _actuatorService.CurrentSpeed;
                
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
        /// Sets the fan to a specific speed manually (manual override).
        /// </summary>
        /// <param name="fanSpeed">The desired fan speed level.</param>
        /// <returns>The result of the manual fan speed setting.</returns>
        [HttpPost("override")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SetManualFanSpeed([FromBody] string fanSpeed)
        {
            try
            {
                if (string.IsNullOrEmpty(fanSpeed))
                {
                    return BadRequest("Fan speed cannot be empty");
                }

                if (!Enum.TryParse<FanSpeed>(fanSpeed, true, out var speed))
                {
                    return BadRequest($"Invalid fan speed: {fanSpeed}. Valid options are: Off, Low, Medium, High");
                }

                _actuatorService.SetManualOverride(speed);
                _cache.Remove("current_fan_speed");
                
                _logger.LogInfo($"Manual fan speed set to: {speed}");

                return Ok(new
                {
                    NewFanSpeed = speed.ToString(),
                    Mode = "Manual",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting manual fan speed to {fanSpeed}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to set manual fan speed.");
            }
        }

        /// <summary>
        /// Gets the current status of the manual override.
        /// </summary>
        /// <returns>The current override status (active/inactive) and fan speed.</returns>
        [HttpGet("overrideStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetManualOverrideStatus()
        {
            try
            {
                bool overrideStatus = _actuatorService.IsManualOverrideActive;
                var currentSpeed = _actuatorService.CurrentSpeed;

                return Ok(new 
                {
                    Active = overrideStatus,
                    CurrentSpeed = currentSpeed.ToString(),
                    Message = overrideStatus ? "Manual override Active" : "System in Auto mode",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving manual override status", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve status.");
            }
        }
    }
}