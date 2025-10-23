namespace smartFan.Services.Interfaces

{
    public interface IActuatorService
    {
        void Update(double temperature);
        ActuatorSimulator.FanSpeed CurrentSpeed { get; }
    }
}