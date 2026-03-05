using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using smartFan.Services.Interfaces;
using System;
using smartFan.Services;

namespace smartFan.Controllers
{
    ///<summary>
    /// creates the intermediate connection between the esp 32 and the server
    /// <summary>
    [ApiController]
    [Route("api/[controller]")]

    public class GatewayController : ControllerBase
    {
        private readonly IActuatorService _actuatorService;
        private readonly ILoggerService _logger;


        /// <summary>
        /// Initializes the <see cref="GatewayController"/>
        /// <summary>

        public GatewayController(IActuatorService actuatorService, ILoggerService logger)
        {
            _actuatorService = actuatorService ?? throw new ArgumentNullException(nameof(actuatorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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


    }
}

