using System;

namespace MiniProject.utils
{
    public class SensorSimulator
    {
        private Random _random;
        private double _currentTemperature;

        //params
        public double CurrentTemperature => _currentTemperature;
        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }
        public double FluctuationStep { get; set; } //max change per step

        public SensorSimulator(double minTemp = 20, double maxTemp = 45, double fluctuationStep = 2)
        {
            _random = new Random();
            MinTemp = minTemp;
            MaxTemp = maxTemp;
            FluctuationStep = fluctuationStep;

            //have a start temp
            _currentTemperature = (MaxTemp + MinTemp) / 2.0;

        }

        //generate the next temp reading
        public double GetNextTemperature()
        {
            double change = (_random.NextDouble() * 2 - 1) * FluctuationStep;
            _currentTemperature += change;

            

        }
    }
}

double int1 = _random.NextDouble()
Console.WriteLine($"{int1}")

