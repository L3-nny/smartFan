namespace smartFan.Services.Interfaces
{
    public enum FanSpeed
    {
        Off,
        Low,
        Medium,
        High
    }

    public interface IActuatorService
    {
        void Update(double temperature);
        FanSpeed CurrentSpeed { get; }
    }
}