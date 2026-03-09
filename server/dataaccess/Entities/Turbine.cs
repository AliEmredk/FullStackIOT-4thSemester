namespace dataaccess.Entities;

public class Turbine
{
    public int Id { get; set; }

    // from payload: "turbineId": "turbine-alpha"
    public string TurbineId { get; set; } = "";

    // from payload: "turbineName": "Alpha"
    public string TurbineName { get; set; } = "";

    // from payload: "farmId": "your-farm-id"
    public string FarmId { get; set; } = "";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public TurbineStatus CurrentStatus { get; set; } = TurbineStatus.Running;

    public DateTimeOffset? LastTelemetryAt { get; set; }

    public List<TelemetryReading> Telemetry { get; set; } = new();
    public List<AlertEvent> Alerts { get; set; } = new();
    public List<TurbineCommand> Commands { get; set; } = new();
}