using smartFan.Models;
using System;

namespace smartFan.Services.Interfaces
{
    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    public interface ILoggerService
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? ex = null);
        void LogSystemState(double temperature, FanSpeed fanSpeed, int deviceId);
        void LogToDatabase(ErrorLog errorLog);
        void SetLogLevel(LogLevel logLevel);
    
    }
}
