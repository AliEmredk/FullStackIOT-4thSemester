namespace dataaccess.Entities;

public enum TurbineCommandAction
{
    SetInterval = 1,
    Stop = 2,
    Start = 3,
    SetPitch = 4
}

public class TurbineCommand
{
    public long Id { get; set; }

    public int TurbineIdFk { get; set; }
    public Turbine Turbine { get; set; } = null!;

    public int UserIdFk { get; set; }
    public AppUser User { get; set; } = null!;

    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    public TurbineCommandAction Action { get; set; }

    // Store original validated payload for full audit history
    // Example: {"action":"setInterval","value":10} etc.
    public string PayloadJson { get; set; } = "";

    // optional: track if command was actually published to broker successfully
    public bool Published { get; set; } = false;

    public DateTimeOffset? PublishedAt { get; set; }
}