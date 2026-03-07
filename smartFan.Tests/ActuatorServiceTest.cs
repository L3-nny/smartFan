using Xunit;
using Moq;
using smartFan.Services;
using smartFan.Services.Interfaces;

namespace smartFan.Tests
{
    public class ActuatorServiceTest
    {
        [Fact]
        public void Update_ShouldSetFanSpeedOff_WhenTemperatureBelow25()
        {
            // Arrange
            var mockLogger = new Mock<ILoggerService>();
            var actuator = new ActuatorService(mockLogger.Object);
            double temperature = 24.0;

            // Act
            actuator.Update(temperature);

            // Assert
            Assert.Equal(FanSpeed.Off, actuator.CurrentSpeed);
        }

        [Fact]
        public void Update_ShouldSetFanSpeedLow_WhenTemperatureBetween25And30()
        {
            // Arrange
            var mockLogger = new Mock<ILoggerService>();
            var actuator = new ActuatorService(mockLogger.Object);
            double temperature = 28.0;

            // Act
            actuator.Update(temperature);

            // Assert
            Assert.Equal(FanSpeed.Low, actuator.CurrentSpeed);
        }

        [Fact]
        public void Update_ShouldSetFanSpeedMedium_WhenTemperatureBetween30And34()
        {
            // Arrange
            var mockLogger = new Mock<ILoggerService>();
            var actuator = new ActuatorService(mockLogger.Object);
            double temperature = 32.0;

            // Act
            actuator.Update(temperature);

            // Assert
            Assert.Equal(FanSpeed.Medium, actuator.CurrentSpeed);
        }

        [Fact]
        public void Update_ShouldSetFanSpeedHigh_WhenTemperatureAbove34()
        {
            // Arrange
            var mockLogger = new Mock<ILoggerService>();
            var actuator = new ActuatorService(mockLogger.Object);
            double temperature = 36.0;

            // Act
            actuator.Update(temperature);

            // Assert
            Assert.Equal(FanSpeed.High, actuator.CurrentSpeed);
        }
    }
}