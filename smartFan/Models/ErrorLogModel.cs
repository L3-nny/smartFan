using System;

namespace smartFan.Models
{
    public class ErrorLogModel
    {
        public int Id { get; set; }
        public int DeviceConfigId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ErrorType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string? ErrorDetails { get; set; }
        public bool Resolved { get; set; }

        //Navigation
        public DeviceConfigModel? DeviceConfig { get; set; }
    }
}
