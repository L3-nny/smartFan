using System;

namespace MiniProject.utils
{
    public class SensorSimulator
    {
        private Random _random;
        private double _currentTemperature;

        //configurable params
        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }
        public double FluctuationStep { get; set; } //max change per step

        public SensorSimulator(double minTemp = 20, double maxTemp = 45, double fluctuationStep = 2)
        {
            _random = new Random();
            MinTemp = minTemp;
            MaxTemp = maxTemp;
            
        }
    }
}

