namespace dataaccess.Entities;

public enum AlertSeverity
{
    Info = 1,
    Warning = 2,
    Critical = 3
}

public class AlertEvent
{
    public long Id { get; set; }

    public int TurbineIdFk { get; set; }
    public Turbine Turbine { get; set; } = null!;

    public DateTimeOffset Timestamp { get; set; }

    public AlertSeverity Severity { get; set; }

    public string Message { get; set; } = "";
}