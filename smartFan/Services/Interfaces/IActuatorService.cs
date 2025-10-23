namespace smartFan.Services.Interfaces

{
    public interface IActuatorService
    {
        void Update(double temperature);
        ActuatorService.FanSpeed CurrentSpeed { get; }
    }
}