using smartFan.Services.Interfaces;

namespace smartFan.Services
{
    public class ActuatorService : IActuatorService
    {
        public smartFan.Services.Interfaces.FanSpeed CurrentSpeed { get; private set; }

        public ActuatorService()
        {
            // Initialize with default off state
            CurrentSpeed = smartFan.Services.Interfaces.FanSpeed.Off;
        }

        public void Update(double temperature)
        {
            if (temperature < 25)
            {
                CurrentSpeed = smartFan.Services.Interfaces.FanSpeed.Off;
            }
            else if (temperature < 30)
            {
                CurrentSpeed = smartFan.Services.Interfaces.FanSpeed.Low;
            }
            else if (temperature < 34)
            {
                CurrentSpeed = smartFan.Services.Interfaces.FanSpeed.Medium;
            }
            else
            {
                CurrentSpeed = smartFan.Services.Interfaces.FanSpeed.High;
            }

            // Simple console logging for now (could be enhanced later)
            Console.WriteLine($"[Actuator] Temp: {temperature:F1}°C --> Fan: {CurrentSpeed}");
        }
    }
}