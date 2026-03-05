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
        private readonly ISensorService _physicalSensorService;


        /// <summary>
        /// Initializes the <see cref="GatewayController"/>
        /// </summary>

        public GatewayController(IActuatorService actuatorService, ILoggerService logger, ISensorService physicalSensorService)
        {
            _actuatorService = actuatorService ?? throw new ArgumentNullException(nameof(actuatorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _physicalSensorService = physicalSensorService ?? throw new ArgumentNullException(nameof(physicalSensorService));
        }

        /// <summary>
        /// Fetch the current fan speed
        /// </summary>
        /// <returns>A JSON object containing the speed as an integer (0-3)</returns>
        [HttpGet("status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult FetchSpeedtoSendtoESP()
        {
            try
            {
                // Get enum value from ActuatorService and convert to integer (0-3)
                var currentSpeed = (int)_actuatorService.CurrentSpeed;

                //Get manual override status as bool and convert to int (0 or 1)
                var manualOverrideStatus = _actuatorService.IsManualOverrideActive ? 1 : 0;

                return Ok(new {
                     s = currentSpeed,
                     m = manualOverrideStatus
                });
            }

            catch (Exception ex)
            {
                _logger.LogError("Error fetching fan speed for ESP", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
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
                _physicalSensorService.RegisterHardwareReading(temperature);
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

