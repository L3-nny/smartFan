using Microsoft.AspNetCore.Mvc;
using smartFan.Services;

namespace smartFan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemperatureLogController : ControllerBase
    {
        private readonly TemperatureLogService _temperatureLogService;
        private readonly ILogger<TemperatureLogController> _logger;

        public TemperatureLogController(TemperatureLogService temperatureLogService, ILogger<TemperatureLogController> logger)
        {
            _temperatureLogService = temperatureLogService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                var logs = await _temperatureLogService.GetAllLogsAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all temperature logs");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("device/{deviceId}")]
        public async Task<IActionResult> GetLogsByDevice(int deviceId)
        {
            try
            {
                var logs = await _temperatureLogService.GetLogsByDeviceAsync(deviceId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving temperature logs for device {DeviceId}", deviceId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}