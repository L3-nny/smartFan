using smartFan.Models;
using smartFan.Repositories.Interfaces;
using smartFan.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace smartFan.Services
{
    public class ErrorLogService
    {
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ILoggerService _logger;

        public ErrorLogService(IErrorLogRepository errorLogRepository, ILoggerService logger)
        {
            _errorLogRepository = errorLogRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ErrorLog>> GetAllErrorsAsync()
        {
            try
            {
                _logger.LogInfo("Retrieving all error logs");
                return await _errorLogRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving error logs", ex);
                throw;
            }
        }

        public async Task<ErrorLog?> GetErrorByIdAsync(int id)
        {
            try
            {
                _logger.LogInfo($"Retrieving error log with ID: {id}");
                return await _errorLogRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving error log with ID {id}", ex);
                throw;
            }
        }

        public async Task<ErrorLog> CreateErrorAsync(ErrorLog errorLog)
        {
            try
            {
                _logger.LogInfo($"Creating new error log: {errorLog.ErrorType}");
                await _errorLogRepository.AddAsync(errorLog);
                return errorLog;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating error log", ex);
                throw;
            }
        }

        public async Task<ErrorLog> UpdateErrorAsync(ErrorLog errorLog)
        {
            try
            {
                _logger.LogInfo($"Updating error log with ID: {errorLog.Id}");
                await _errorLogRepository.UpdateAsync(errorLog);
                return errorLog;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating error log with ID {errorLog.Id}", ex);
                throw;
            }
        }

        public async Task DeleteErrorAsync(int id)
        {
            try
            {
                _logger.LogInfo($"Deleting error log with ID: {id}");
                await _errorLogRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting error log with ID {id}", ex);
                throw;
            }
        }

        public async Task MarkAsResolvedAsync(int id)
        {
            try
            {
                var errorLog = await _errorLogRepository.GetByIdAsync(id);
                if (errorLog != null)
                {
                    errorLog.Resolved = true;
                    await _errorLogRepository.UpdateAsync(errorLog);
                    _logger.LogInfo($"Marked error log {id} as resolved");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking error log {id} as resolved", ex);
                throw;
            }
        }
    }
}
