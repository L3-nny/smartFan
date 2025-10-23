//do I want to log each instance a new temp is generated, a temp that changes the fan state, fan condition
//do I need to see the flow at each step and log that too?
//do I need to check the current fan state --api --> CurrentSpeed method
//do I need to know the current temp --api-->

//guidelines
//use _camelCase for private fields--so stuff like _currentTemperature 
using System;
using System.IO;

namespace smartFan.Services
{
    public class Logger
    {
        private readonly string _logFilePath;

        public Logger(string logFilePath = "logs.txt")
        {
            _logFilePath = logFilePath;

            //Ensure file exists
            if (!File.Exists(_logFilePath))
            {
                File.Create(_logFilePath).Dispose();
            }
        }

        ///Logs a message with a timestamp to console and file
        /// 
        public void Log(string message)
        {
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

            //Console output
            Console.WriteLine(entry);

            //Append to log file
            File.AppendAllText(_logFilePath, entry + Environment.NewLine);

        }

        //log temp and fan speed
        public void LogSystemState(double temperature, ActuatorService.FanSpeed fanSpeed)
        {
            string state = $"Temp: {temperature:F1}°C, Fan: {fanSpeed}";
            Log(state);
        }
    } 
    
}