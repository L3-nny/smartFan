namespace smartFan.Models
{
    public class DeviceConfigModel
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
        public ICollection<TemperatureLogModel> TemperatureLogs { get; set; } = new List<TemperatureLogModel>();
        public ManualOverrideModel? ManualOverride { get; set; }
        public ICollection<ErrorLogModel> ErrorLogs { get; set; } = new List<ErrorLogModel>();

    }
}