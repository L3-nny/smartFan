using System;

namespace smartFan.Models
{

    public class TemperatureLogModel
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public string FanSpeed { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string LogLevel { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        //Navigation property
        public DeviceConfigModel? DeviceConfig { get; set; }
        public int DeviceConfigId { get; set; }
    }

}