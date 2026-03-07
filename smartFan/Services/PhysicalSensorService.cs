using smartFan.Services.Interfaces;

namespace smartFan.Services
{
    public class PhysicalSensorService : ISensorService 

    {
        //Configuration properties that can be ued to validate ESP32 data
        public double MinTemp { get; set; } = 10.0;
        public double MaxTemp { get; set; } = 53.0;
        public double FluctuationStep { get; set; }

        //Current temperature reading from the ESP32
        private double _currentTemperature;

        //Read only getter for the current temperature
        public double CurrentTemperature => _currentTemperature;

        //Returns the latest actual reading
        public double GetNextTemperature()
        {
            return _currentTemperature;
        }
        //private double _currentHumidity;

        //method to be called by new HardwareGatewayController
        public void RegisterHardwareReading(double temperatureReading) //, double humidity)
        {
            if (temperatureReading < MinTemp) 
            {
                temperatureReading = MinTemp;
            }
            else if (temperatureReading > MaxTemp) 
            {
                
                temperatureReading = MaxTemp;
            }
            else
            {
            _currentTemperature = temperatureReading;
            }

        }
    }
}