using smartFan.Services.Interfaces;

namespace smartFan.Services
{
    public class ActuatorService : IActuatorService
    {
        private readonly ILoggerService _logger;

        public enum FanSpeed
        {
            Off,
            Low,
            Medium,
            High
        }

        public FanSpeed CurrentSpeed { get; private set; }

        public ActuatorService(ILoggerService logger)
        {
            _logger = logger;
        }

        public void Update(double temperature)
        {
            if (temperature < 25)
            {
                CurrentSpeed = FanSpeed.Off;
            }
            else if (temperature < 30)
            {
                CurrentSpeed = FanSpeed.Low;
            }
            else if (temperature < 34)
            {
                CurrentSpeed = FanSpeed.Medium;
            }
            else
            {
                CurrentSpeed = FanSpeed.High;
            }

            _logger.Log($"[Actuator] Temp: {temperature:F1}°C --> Fan: {CurrentSpeed}");
        }
    }
}