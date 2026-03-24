namespace smartFan.Services;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using smartFan.Models.DTOs;
using smartFan.Services.Interfaces;
using System.Text;
using System.Text.Json;

public class MqttBackgroundService : BackgroundService
{
    private readonly IManagedMqttClient _mqttClient;
    private readonly ISensorService _sensorService;
    private readonly ILogger<MqttBackgroundService> _logger;

    public MqttBackgroundService(ISensorService sensorService, ILogger<MqttBackgroundService> logger)
    {
        _sensorService = sensorService;
        _logger = logger;
        _mqttClient = new MqttFactory().CreateManagedMqttClient();

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttOptions = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(new MqttClientOptionsBuilder()
                .WithClientId("SmartFanBackend")
                .WithTcpServer("localhost", 1883)
                .WithCredentials("", "")
                .Build())
            .Build();

        _mqttClient.ConnectedAsync += e =>
        {
            _logger.LogInformation("Connected to MQTT broker.");
            return Task.CompletedTask;
        };


        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                _logger.LogInformation($"Received MQTT message on {e.ApplicationMessage.Topic}: {payload}");

                var data = JsonSerializer.Deserialize<TelemetryModel>(payload);

                if (data == null)
                {
                    _logger.LogWarning("Failed to deserialize MQTT payload.");
                    return;
                }

                if (!data.Temperature.HasValue)
                {
                    _logger.LogWarning("Temperature value is missing in the MQTT message.");
                    return;
                }

                _logger.LogInformation($"Parsed Temperature: {data.Temperature.Value}");
                _sensorService.RegisterHardwareReading(data.Temperature.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing MQTT message.");
            }
        };

        await _mqttClient.SubscribeAsync("smartFan/telemetry");
        await _mqttClient.StartAsync(mqttOptions);

        await Task.Delay(-1, stoppingToken);
    }
}
