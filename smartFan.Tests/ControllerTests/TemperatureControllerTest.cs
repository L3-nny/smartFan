using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using smartFan.Controllers;
using smartFan.Services.Interfaces;
using System;
using System.Text.Json;

namespace smartFan.Tests.ControllerTests
{
    public class TemperatureControllerTest
    {
        private readonly Mock<ISensorService> _mockSensorService;
        private readonly Mock<ILoggerService> _mockLoggerService;
        private readonly TemperatureController _controller;

        public TemperatureControllerTest()
        {
            _mockSensorService = new Mock<ISensorService>();
            _mockLoggerService = new Mock<ILoggerService>();
            _controller = new TemperatureController(_mockSensorService.Object, _mockLoggerService.Object);
        }

        [Fact]
        public void GetCurrentTemperature_ReturnsOkResult_WithTemperature()
        {
            //Arrange
            _mockSensorService.Setup(s => s.GetNextTemperature()).Returns(26.5);

            //Act
            var result = _controller.GetCurrentTemperature();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var jsonString = JsonSerializer.Serialize(okResult.Value);
            Assert.Contains("26.5", jsonString);
            Assert.Contains("Temperature", jsonString);

             // Verify logger is called (optional)
            _mockLoggerService.Verify(l => l.LogInfo(It.IsAny<string>()), Times.AtLeastOnce);

        }

        [Fact]
        public void GetCurrentTemperature_Returns500Result_WhenExceptionThrown()
        {
            //Arrange
            _mockSensorService.Setup(s => s.GetNextTemperature()).Throws(new Exception("Sensor Error"));

            //Act
            var result = _controller.GetCurrentTemperature();

            //Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("Failed to read temperature", objectResult.Value!.ToString());

            // Verify logger is called for error
            _mockLoggerService.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.AtLeastOnce);

        }
    }
}