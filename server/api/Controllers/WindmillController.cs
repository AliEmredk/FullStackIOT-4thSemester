using Mqtt.Controllers;
using System.Text.Json;
using api.dtos;

namespace api.Controllers;

//Checkout what this ILogger is doing
public class WindmillMqttController(ILogger<WindmillMqttController> logger) : MqttController

{
    [MqttRoute("farm/FullStackIOT-ELK/windmill/{turbineId}/telemetry")]
    public Task HandleTelemetry(string turbineId, WindmillTelemetryDto data)
    {
        if (!string.Equals(turbineId, data.TurbineId, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("Telemetry turbineId mismatch. topic={TopicId} payload={PayloadId}",
                turbineId, data.TurbineId);
            return Task.CompletedTask;
        }
        logger.LogInformation("Telemetry topic turbineId={TurbineId} payload={Payload}",
            turbineId, JsonSerializer.Serialize(data));

        return Task.CompletedTask;
    }

    [MqttRoute("farm/FullStackIOT-ELK/windmill/{turbineId}/alert")]
    public Task HandleAlert(string turbineId, WindmillAlertDto data)
    {
        logger.LogWarning("Alert topic turbineId={TurbineId} payload={Payload}",
            turbineId, JsonSerializer.Serialize(data));

        return Task.CompletedTask;
    }
}

