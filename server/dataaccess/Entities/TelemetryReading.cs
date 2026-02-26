namespace dataaccess.Entities;

public enum TurbineStatus
{
    Running = 1,
    Stopped = 2
}

public class TelemetryReading
{
    public long Id { get; set; }

    public int TurbineIdFk { get; set; }
    public Turbine Turbine { get; set; } = null!;

    public DateTimeOffset Timestamp { get; set; }

    public double WindSpeed { get; set; }
    public double WindDirection { get; set; }
    public double AmbientTemperature { get; set; }
    public double RotorSpeed { get; set; }
    public double PowerOutput { get; set; }
    public double NacelleDirection { get; set; }
    public double BladePitch { get; set; }
    public double GeneratorTemp { get; set; }
    public double GearboxTemp { get; set; }
    public double Vibration { get; set; }

    public TurbineStatus Status { get; set; }
}