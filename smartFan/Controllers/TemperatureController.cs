using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using smartFan.Services.Interfaces;
using System;

namespace smartFan.Controllers
{
    /// <summary>
    /// Provides live temperature readings from the sensor simulator.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TemperatureController : ControllerBase
    {
        private readonly ISensorService _sensorService;
        private readonly ILoggerService _logger;

        /// <summary>
        /// Initializes the <see cref="TemperatureController"/>.
        /// </summary>
        public TemperatureController(ISensorService sensorService, ILoggerService logger)
        {
            _sensorService = sensorService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the current live temperature reading from the sensor simulator.
        /// </summary>
        /// <returns>The latest temperature value in Celsius.</returns>
        [HttpGet("current")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCurrentTemperature()
        {
            try
            {
                double temp = _sensorService.GetNextTemperature();
                _logger.LogInfo($"Live temperature reading: {temp:F2}°C");

                return Ok(new
                {
                    Temperature = temp,
                    Unit = "°C",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reading temperature.", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to read temperature.");
            }
        }
    }
}
