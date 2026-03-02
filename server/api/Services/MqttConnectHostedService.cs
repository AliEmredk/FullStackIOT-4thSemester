using Mqtt.Controllers;

namespace api.Services;

public class MqttConnectHostedService(
    IMqttClientService mqtt,
    ILogger<MqttConnectHostedService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // connect FIRST (so MqttControllerHostedService can subscribe safely)
        await mqtt.ConnectAsync("broker.hivemq.com", 1883);
        logger.LogInformation("✅ MQTT connected to broker.hivemq.com:1883");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}