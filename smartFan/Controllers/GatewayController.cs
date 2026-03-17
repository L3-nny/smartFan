using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using smartFan.Services.Interfaces;
using System;
using smartFan.Services;
using smartFan.Models.DTOs;

namespace smartFan.Controllers
{
    ///<summary>
    /// creates the intermediate connection between the esp 32 and the server
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]

    public class GatewayController : ControllerBase
    {
        private readonly IActuatorService _actuatorService;
        private readonly ILoggerService _logger;
        private readonly ISensorService _sensorService;


        /// <summary>
        /// Initializes the <see cref="GatewayController"/>
        /// </summary>

        public GatewayController(IActuatorService actuatorService, ILoggerService logger, ISensorService sensorService)
        {
            _actuatorService = actuatorService ?? throw new ArgumentNullException(nameof(actuatorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sensorService = sensorService ?? throw new ArgumentNullException(nameof(sensorService));
        }



        /// <summary>
        /// Receives temperature readings from the ESP32 and updates the sensor service.
        /// </summary>
        /// <param name="data">The telemetry data sent by the ESP32.</param>
        /// <returns>Acknowledgment of the received data.</returns>
        [HttpPost("telemetry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult ReceiveTelemetry([FromBody] TelemetryModel data)
        {
            if (data == null || !ModelState.IsValid)
            {
                _logger.LogWarning("Invalid JSON format received from ESP");
                return BadRequest("Invalid data format");
            }

            try
            {
                // Update the sensor service with the new temperature reading
                if (!data.Temperature.HasValue)
                {
                    _logger.LogWarning("Telemetry data received without temperature value");
                    return BadRequest("Temperature value is required");
                }

                double temperature = data.Temperature.Value;

                //Clean the data and save it in the system memory
                _sensorService.RegisterHardwareReading(temperature);
                //Pass the temperature to the actuator service to update the fan speed accordingly
                _actuatorService.Update(temperature);

                //Send response back including the new fan speed
                return Ok(new { 
                    s = (int)_actuatorService.CurrentSpeed,
                    m = _actuatorService.IsManualOverrideActive ? 1 : 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing telemetry data from ESP", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

    }
}

