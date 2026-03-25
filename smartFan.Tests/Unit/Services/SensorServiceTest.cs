using Xunit;
using smartFan.Services;
using Moq;
using smartFan.Services.Interfaces;

namespace smartFan.Tests
{
    public class SensorServiceTest
    {
        [Fact]
        public void GetNextTemperature_ShouldReturnSameTemperature_WhenRandomBelowThreshold()
        {
            //Arrange
            var mockRandom = new Mock<IRandomProvider>();
            mockRandom.Setup(r => r.NextDouble()).Returns(0.1); //triggers 20%
            var sensor = new SensorSimulator(mockRandom.Object);
            
            //Act
            double initialTemp = sensor.CurrentTemperature;
            double nextTemp = sensor.GetNextTemperature();
            
            //Assert
            Assert.Equal(initialTemp, nextTemp);
        }
    }
}