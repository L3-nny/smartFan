namespace smartFan.Services.Interfaces
{
    public interface ISensorService
    {
        //Properties that describe the sensor's configurations
        double MinTemp { get; set; }
        double MaxTemp { get; set; }
        double FluctuationStep { get; set; }

        //read only property to get the current temperature
        double CurrentTemperature { get; }

        //method to simulate the next temperature reading
        double GetNextTemperature();

    }
    
}