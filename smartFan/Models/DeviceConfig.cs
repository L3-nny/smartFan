using System;

namespace smartFan.Models
{
    public class DeviceConfig
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }
        public double FluctuationStep { get; set; }
        public double ThresholdLow { get; set; }
        public double ThresholdMedium { get; set; }
        public double ThresholdHigh { get; set; }
        public bool AutoModeEnabled { get; set; }

        //Navigation properties
        public ICollection<TemperatureLog> TemperatureLogs { get; set; }
        public ManualOverride ManualOverride { get; set; }
        public ICollection<ErrorLog> ErrorLogs { get; set; }

    }
}