using System;

namespace smartFan.Models
{
    public class FanSettingsModel
    {
        public double ThresholdOff { get; set; } 
        public double ThresholdLow { get; set; }
        public double ThresholdMedium { get; set; }

    }
}