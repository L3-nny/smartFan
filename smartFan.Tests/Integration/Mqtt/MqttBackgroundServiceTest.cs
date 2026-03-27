using Xunit;
using Moq;
using MQTTnet;
using MQTTnet.Packets;
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

        [Fact]
        public async Task Should_Update_SensorService_When_Valid_Mqtt_Payload_Is_Received()
        {
            // 1. Arrange: Create the service under test
            // Note: In a real app, you'd inject the Host/Port from config. 
            // For now, LENNY NOTE!!!ensure your Ubuntu Docker is accessible via localhost.
            var service = new MqttBackgroundService(_mockSensorService.Object, _mockLogger.Object);
            using var cts = new CancellationTokenSource();

            // Start the service logic
            await service.StartAsync(cts.Token);

            // 2. Act: Setup a "Fake ESP32" (Test Client) to send data
            var mqttFactory = new MqttFactory();
            using var testClient = mqttFactory.CreateMqttClient();
            
            // Connect to the Dynamic Port provided by our Docker Fixture
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_fixture.Host, _fixture.Port) 
                .Build();

            await testClient.ConnectAsync(options, cts.Token);

