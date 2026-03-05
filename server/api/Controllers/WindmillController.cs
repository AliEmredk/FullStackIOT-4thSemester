using Mqtt.Controllers;
using System.Text.Json;
using api.dtos;
using api.Services;
using api.Services;

namespace api.Controllers;

//Checkout what this ILogger is doing
public class WindmillMqttController(
    ILogger<WindmillMqttController> logger,
    IWindmillTelemetryService telemetryService, TelemetryCache cache
    ) : MqttController

{
    [MqttRoute("farm/FullStackIOT-ELK/windmill/{turbineId}/telemetry")]
    public async Task HandleTelemetry(string turbineId, WindmillTelemetryDto data)
    {
        if (!string.Equals(turbineId, data.TurbineId, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("Telemetry turbineId mismatch. topic={TopicId} payload={PayloadId}",
                turbineId, data.TurbineId);
            return;
        }
        
        cache.Upsert(data);
        

        logger.LogInformation("Telemetry topic turbineId={TurbineId} payload={Payload}",
            turbineId, JsonSerializer.Serialize(data));

        await telemetryService.SaveTelemetryAsync(data);
    }

    [MqttRoute("farm/FullStackIOT-ELK/windmill/{turbineId}/alert")]
    public async Task HandleAlert(string turbineId, WindmillAlertDto data)
    {
        logger.LogWarning("Alert topic turbineId={TurbineId} payload={Payload}",
            turbineId, JsonSerializer.Serialize(data));

        await telemetryService.SaveAlertAsync(data);
    }
}

