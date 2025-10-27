using smartFan.Models;
using smartFan.Repositories.Interfaces;
using smartFan.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace smartFan.Services
{
    public class TemperatureLogService
    {
        private readonly ITemperatureLogRepository _temperatureLogRepository;
        private readonly ILoggerService _logger;

        public TemperatureLogService(ITemperatureLogRepository temperatureLogRepository, ILoggerService logger)
        {
            _temperatureLogRepository = temperatureLogRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TemperatureLog>> GetAllLogsAsync()
        {
            try
            {
                _logger.LogInfo("Retrieving all temperature logs");
                return await _temperatureLogRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving temperature logs", ex);
                throw;
            }
        }

        public async Task<TemperatureLog?> GetLogByIdAsync(int id)
        {
            try
            {
                _logger.LogInfo($"Retrieving temperature log with ID: {id}");
                return await _temperatureLogRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving temperature log with ID {id}", ex);
                throw;
            }
        }

        public async Task<TemperatureLog> CreateLogAsync(TemperatureLog temperatureLog)
        {
            try
            {
                _logger.LogInfo($"Creating new temperature log: {temperatureLog.Temperature}°C");
                await _temperatureLogRepository.AddAsync(temperatureLog);
                return temperatureLog;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating temperature log", ex);
                throw;
            }
        }

        public async Task<TemperatureLog> UpdateLogAsync(TemperatureLog temperatureLog)
        {
            try
            {
                _logger.LogInfo($"Updating temperature log with ID: {temperatureLog.Id}");
                await _temperatureLogRepository.UpdateAsync(temperatureLog);
                return temperatureLog;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating temperature log with ID {temperatureLog.Id}", ex);
                throw;
            }
        }

        public async Task DeleteLogAsync(int id)
        {
            try
            {
                _logger.LogInfo($"Deleting temperature log with ID: {id}");
                await _temperatureLogRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting temperature log with ID {id}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<TemperatureLog>> GetLogsByDeviceAsync(int deviceId)
        {
            try
            {
                _logger.LogInfo($"Retrieving temperature logs for device: {deviceId}");
                return await _temperatureLogRepository.GetByDeviceIdAsync(deviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving logs for device {deviceId}", ex);
                throw;
            }
        }


    }
}
