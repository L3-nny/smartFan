using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using smartFan.Models;
using smartFan.Repositories.Interfaces;
using smartFan.Services.Interfaces;
using LogLevel = smartFan.Services.Interfaces.LogLevel;

namespace smartFan.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IHostEnvironment _env;
        private readonly string _logDirectory;
        private readonly bool _consoleLoggingEnabled;
        private readonly object _lockObj = new();
        private LogLevel _currentLevel;

        public LoggerService(IErrorLogRepository errorLogRepository, IHostEnvironment env, IConfiguration config)
        {
            _errorLogRepository = errorLogRepository;
            _env = env;

            // Load log settings from configuration
            _logDirectory = config["LogSettings:Directory"] ?? "utils/Logs";
            _consoleLoggingEnabled = bool.TryParse(config["LogSettings:EnableConsole"], out bool enableConsole)
                ? enableConsole
                : true;

            // Default log level (configurable)
            if (!Enum.TryParse(config["LogSettings:Level"], true, out _currentLevel))
                _currentLevel = LogLevel.Info;

            Directory.CreateDirectory(_logDirectory);
        }

        public void SetLogLevel(LogLevel level)
        {
            _currentLevel = level;
            LogInfo($"Log level changed to {_currentLevel}");
        }

        public void LogInfo(string message) => Log(LogLevel.Info, message);
        public void LogWarning(string message) => Log(LogLevel.Warning, message);
        public void LogDebug(string message) => Log(LogLevel.Debug, message);
        public void LogCritical(string message) => Log(LogLevel.Critical, message);

        public void LogError(string message, Exception? ex = null)
        {
            string errorMessage = ex == null ? message : $"{message} | Exception: {ex.Message}";
            Log(LogLevel.Error, errorMessage);

            var errorLog = new ErrorLog
            {
                Message = message,
                ErrorMessage = message,
                ErrorDetails = ex?.StackTrace,
                ErrorType = LogLevel.Error.ToString(),
                Timestamp = DateTime.UtcNow,
                DeviceConfigId = 1, // Default device ID
                Resolved = false
            };

            _ = LogToDatabaseAsync(errorLog);
        }

        public void LogSystemState(double temperature, FanSpeed fanSpeed, int deviceId)
        {
            string systemState = $"[Device {deviceId}] Temp: {temperature:F1}°C | Fan: {fanSpeed}";
            Log(LogLevel.Info, systemState);
        }

        public void LogToDatabase(ErrorLog errorLog)
        {
            _ = LogToDatabaseAsync(errorLog);
        }

        private async Task LogToDatabaseAsync(ErrorLog errorLog)
        {
            try
            {
                await _errorLogRepository.AddAsync(errorLog);
            }
            catch (Exception ex)
            {
                Log(LogLevel.Warning, $"Failed to write error log to DB: {ex.Message}");
            }
        }

        private void Log(LogLevel level, string message)
        {
            if (level < _currentLevel) return;

            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            string entry = $"[{timestamp}] [{level}] [{_env.EnvironmentName}] {message}";
            string logFilePath = Path.Combine(_logDirectory, $"app-{DateTime.UtcNow:yyyy-MM-dd}.txt");

            lock (_lockObj)
            {
                File.AppendAllText(logFilePath, entry + Environment.NewLine);
            }

            if (_consoleLoggingEnabled)
            {
                Console.ForegroundColor = level switch
                {
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    LogLevel.Critical => ConsoleColor.DarkRed,
                    LogLevel.Debug => ConsoleColor.Cyan,
                    _ => ConsoleColor.Gray
                };

                Console.WriteLine(entry);
                Console.ResetColor();
            }
        }
    }
}
