using System;
using TempControl.Services.Interfaces;

namespace TempControl.Services
{
    public class SensorSimulator : ISensorService
    {
        private readonly IRandomProvider _random;
        private double _currentTemperature;

        //params
        public double CurrentTemperature => _currentTemperature;
        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }
        public double FluctuationStep { get; set; } //max change per step

        public SensorSimulator(IRandomProvider randomProvider, double minTemp = 20, double maxTemp = 45, double fluctuationStep = 2)
        {
            _random = randomProvider;
            MinTemp = minTemp;
            MaxTemp = maxTemp;
            FluctuationStep = fluctuationStep;

            //have a start temp
            _currentTemperature = (MaxTemp + MinTemp) / 2.0;

        }

        //generate the next temp reading
        public double GetNextTemperature()
        {

            //clamp to within range
            //add a small probability that temp stays the same
            if (_random.NextDouble() < 0.2)
            {
                return _currentTemperature;
            }
            else
            {
                double change = (_random.NextDouble() * 2 - 1) * FluctuationStep;
                _currentTemperature += change;

                if (_currentTemperature < MinTemp)
                {
                    _currentTemperature = MinTemp;
                }
                else if (_currentTemperature > MaxTemp)
                {
                    _currentTemperature = MaxTemp;
                }
                return _currentTemperature;



            }
        }
    }
}

