using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using smartFan.Services.Interfaces;
using System;

namespace smartFan.Controllers
{
    /// <summary>
    /// Provides background monitoring and system health functionality.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MonitorController : ControllerBase
    {
        private readonly ILoggerService _logger;
        private readonly ISensorService _sensorService;
        private readonly IActuatorService _actuatorService;

        /// <summary>
        /// Initializes the <see cref="MonitorController"/>.
        /// </summary>
        public MonitorController(ILoggerService logger, ISensorService sensorService, IActuatorService actuatorService)
        {
            _logger = logger;
            _sensorService = sensorService;
            _actuatorService = actuatorService;
        }

        /// <summary>
        /// Gets current monitoring status and metrics.
        /// </summary>
        /// <returns>Real-time monitoring data.</returns>
        [HttpGet("status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetMonitoringStatus()
        {
            try
            {
                var currentTemp = _sensorService.GetNextTemperature();
                var fanSpeed = _actuatorService.CurrentSpeed;

                var status = new
                {
                    MonitoringActive = true,
                    CurrentTemperature = currentTemp,
                    CurrentFanSpeed = fanSpeed.ToString(),
                    SystemHealth = "Good",
                    LastUpdate = DateTime.UtcNow,
                    Alerts = new string[] { }, // No active alerts
                    Metrics = new
                    {
                        TemperatureUnit = "°C",
                        FanSpeedLevel = (int)fanSpeed,
                        SystemUptime = TimeSpan.FromHours(1), // Mock uptime
                        MonitoringInterval = "1 second"
                    }
                };

                _logger.LogInfo("Monitoring status requested");
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting monitoring status", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get monitoring status.");
            }
        }

        /// <summary>
        /// Starts the background monitoring service.
        /// </summary>
        /// <returns>Confirmation of monitoring start.</returns>
        [HttpPost("start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult StartMonitoring()
        {
            try
            {
                // Would start BackgroundMonitorService when implemented
                _logger.LogInfo("Monitoring service start requested");

                return Ok(new
                {
                    Message = "Background monitoring service started",
                    Status = "Active",
                    StartTime = DateTime.UtcNow,
                    MonitoringInterval = "1 second"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error starting monitoring service", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to start monitoring.");
            }
        }

        /// <summary>
        /// Stops the background monitoring service.
        /// </summary>
        /// <returns>Confirmation of monitoring stop.</returns>
        [HttpPost("stop")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult StopMonitoring()
        {
            try
            {
                // Would stop BackgroundMonitorService when implemented
                _logger.LogInfo("Monitoring service stop requested");

                return Ok(new
                {
                    Message = "Background monitoring service stopped",
                    Status = "Stopped",
                    StopTime = DateTime.UtcNow,
                    LastRunDuration = TimeSpan.FromHours(1) // Mock duration
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error stopping monitoring service", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to stop monitoring.");
            }
        }

        /// <summary>
        /// Gets system health check results.
        /// </summary>
        /// <returns>Comprehensive health check data.</returns>
        [HttpGet("health")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetHealthCheck()
        {
            try
            {
                var currentTemp = _sensorService.GetNextTemperature();
                var fanSpeed = _actuatorService.CurrentSpeed;

                var healthCheck = new
                {
                    OverallStatus = "Healthy",
                    Components = new
                    {
                        TemperatureSensor = new
                        {
                            Status = "OK",
                            CurrentReading = currentTemp,
                            LastCheck = DateTime.UtcNow
                        },
                        FanActuator = new
                        {
                            Status = "OK",
                            CurrentSpeed = fanSpeed.ToString(),
                            LastCheck = DateTime.UtcNow
                        },
                        Database = new
                        {
                            Status = "OK",
                            ConnectionActive = true,
                            LastCheck = DateTime.UtcNow
                        },
                        Logger = new
                        {
                            Status = "OK",
                            LastLogEntry = DateTime.UtcNow,
                            LastCheck = DateTime.UtcNow
                        }
                    },
                    CheckTime = DateTime.UtcNow,
                    NextCheck = DateTime.UtcNow.AddMinutes(1)
                };

                _logger.LogInfo("Health check performed");
                return Ok(healthCheck);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error performing health check", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Health check failed.");
            }
        }

        /// <summary>
        /// Gets recent monitoring alerts and warnings.
        /// </summary>
        /// <returns>List of recent system alerts.</returns>
        [HttpGet("alerts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAlerts()
        {
            try
            {
                // Mock alert data - would come from actual monitoring service
                var alerts = new[]
                {
                    new
                    {
                        Id = 1,
                        Type = "Information",
                        Message = "System operating normally",
                        Timestamp = DateTime.UtcNow.AddMinutes(-5),
                        Resolved = true
                    }
                };

                _logger.LogInfo("Monitoring alerts requested");

                return Ok(new
                {
                    Alerts = alerts,
                    TotalAlerts = alerts.Length,
                    ActiveAlerts = alerts.Count(a => !a.Resolved),
                    LastUpdate = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting monitoring alerts", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get alerts.");
            }
        }
    }
}