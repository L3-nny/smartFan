using System;

namespace smartFan.Models
{

    public class TemperatureLog
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public string FanSpeed { get; set; }
        public string Source { get; set; }
        public string LogLevel { get; set; }
        public string Message { get; set; }

        //Navigation property
        public DeviceConfig DeviceConfig { get; set; }
        public int DeviceConfigId { get; set; }
    }

}