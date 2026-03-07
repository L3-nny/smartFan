using System;

namespace smartFan.Models
{
    public class ManualOverrideModel
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int DeviceConfigId { get; set; }
        public string FanSpeed { get; set; } = string.Empty;
        public DateTime OverrideUntil { get; set; }
        public bool Active { get; set; }

        //Navigation
        public DeviceConfigModel? DeviceConfig { get; set; }
    }
}