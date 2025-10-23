using System;

namespace TempControl.Services
{
    public class ActuatorSimulator
    {
        public enum FanSpeed
        {
            Off,
            Low,
            Medium,
            High
        }

        public FanSpeed CurrentSpeed { get; private set; }

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

            Console.WriteLine($"[Actuator] Temp: {temperature:F1}°C --> Fan: {CurrentSpeed}");



        }
    }
}