using Xunit;
using Moq;
using MQTTnet;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using smartFan.Services;
using smartFan.Services.Interfaces;
using smartFan.Models.DTOs;
using smartFan.Tests.Fixtures;

namespace smartFan.Tests.Integration.Mqtt
{
    public class MqttBackgroundServiceTests : IClassFixture<MosquittoFixture>
    {
        private readonly MosquittoFixture _fixture;
        private readonly Mock<ISensorService> _mockSensorService;
        private readonly Mock<ILogger<MqttBackgroundService>> _mockLogger;

        public MqttBackgroundServiceTests(MosquittoFixture fixture)
        {
            _fixture = fixture;
            _mockSensorService = new Mock<ISensorService>();
            _mockLogger = new Mock<ILogger<MqttBackgroundService>>();
        }

