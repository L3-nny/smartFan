using System;

namespace smartFan.Models
{
    public class ManualOverride
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string FanSpeed { get; set; }
        public DateTime OverrideUntil { get; set; }
        public bool Active { get; set; }

        //Navigation
        public DeviceConfig DeviceConfig { get; set; }
    }
}