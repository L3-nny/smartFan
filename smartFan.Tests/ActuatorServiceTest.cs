using Xunit;
using TempControl.Services;

namespace smartFan.Tests
{
    public class ActuatorServiceTest
    {
        [Fact]
        public void Update_ShouldSetFanSpeedOff_WhenTemperatureBelow25()
        {
            // Arrange
            var actuator = new ActuatorSimulator();
            double temperature = 24.0;

            // Act
            actuator.Update(temperature);

            // Assert
            Assert.Equal(ActuatorSimulator.FanSpeed.Off, actuator.CurrentSpeed);
        }

        [Fact]
        public void Update_ShouldSetFanSpeedLow_WhenTemperatureBetween25And30()
        {
            // Arrange
            var actuator = new ActuatorSimulator();
            double temperature = 28.0;

            // Act
            actuator.Update(temperature);

            // Assert
            Assert.Equal(ActuatorSimulator.FanSpeed.Low, actuator.CurrentSpeed);
        }

        [Fact]
        public void Update_ShouldSetFanSpeedMedium_WhenTemperatureBetween30And34()
        {
            // Arrange
            var actuator = new ActuatorSimulator();
            double temperature = 32.0;

            // Act
            actuator.Update(temperature);

            // Assert
            Assert.Equal(ActuatorSimulator.FanSpeed.Medium, actuator.CurrentSpeed);
        }

        [Fact]
        public void Update_ShouldSetFanSpeedHigh_WhenTemperatureAbove34()
        {
            // Arrange
            var actuator = new ActuatorSimulator();
            double temperature = 36.0;

            // Act
            actuator.Update(temperature);

            // Assert
            Assert.Equal(ActuatorSimulator.FanSpeed.High, actuator.CurrentSpeed);
        }
    }
}