using System;

namespace smartFan.Models
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public int DeviceConfigId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ErrorType { get; set; }
        public string Message { get; set; }
        public bool Resolved { get; set; }

        //Navigation
        public DeviceConfig DeviceConfig { get; set; }
    }
}
