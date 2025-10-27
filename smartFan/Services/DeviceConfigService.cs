using Microsoft.Extensions.Caching.Memory;
using smartFan.Models;
using smartFan.Repositories.Interfaces;
using smartFan.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace smartFan.Services
{
    public class DeviceConfigService
    {
        private readonly IDeviceConfigRepository _deviceConfigRepository;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;

        public DeviceConfigService(IDeviceConfigRepository deviceConfigRepository, ILoggerService logger, IMemoryCache cache)
        {
            _deviceConfigRepository = deviceConfigRepository;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<DeviceConfig>> GetAllConfigsAsync()
        {
            try
            {
                const string cacheKey = "all_device_configs";
                
                // Use GetOrCreateAsync for async operations
                var result = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    // Cache device configs for 5 minutes since they don't change often
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    
                    _logger.LogInfo("Retrieving fresh device configurations from repository");
                    return await _deviceConfigRepository.GetAllAsync();
                });
                
                return result ?? new List<DeviceConfig>();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving device configurations", ex);
                throw;
            }
        }

        public async Task<DeviceConfig?> GetConfigByIdAsync(int id)
        {
            try
            {
                _logger.LogInfo($"Retrieving device configuration with ID: {id}");
                return await _deviceConfigRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving device configuration with ID {id}", ex);
                throw;
            }
        }

        public async Task<DeviceConfig> CreateConfigAsync(DeviceConfig deviceConfig)
        {
            try
            {
                _logger.LogInfo($"Creating new device configuration for device: {deviceConfig.DeviceId}");
                await _deviceConfigRepository.AddAsync(deviceConfig);
                
                // Invalidate cache since we added new data
                _cache.Remove("all_device_configs");
                _logger.LogInfo("Device configurations cache invalidated after creation");
                
                return deviceConfig;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating device configuration", ex);
                throw;
            }
        }

        public async Task<DeviceConfig> UpdateConfigAsync(DeviceConfig deviceConfig)
        {
            try
            {
                _logger.LogInfo($"Updating device configuration with ID: {deviceConfig.Id}");
                await _deviceConfigRepository.UpdateAsync(deviceConfig);
                
                // Invalidate cache since we updated data
                _cache.Remove("all_device_configs");
                _logger.LogInfo("Device configurations cache invalidated after update");
                
                return deviceConfig;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating device configuration with ID {deviceConfig.Id}", ex);
                throw;
            }
        }

        public async Task DeleteConfigAsync(int id)
        {
            try
            {
                _logger.LogInfo($"Deleting device configuration with ID: {id}");
                await _deviceConfigRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting device configuration with ID {id}", ex);
                throw;
            }
        }
    }
}
