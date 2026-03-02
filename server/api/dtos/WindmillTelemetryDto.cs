namespace api.dtos;

public record WindmillTelemetryDto(
    string TurbineId,
    string TurbineName,
    string FarmId,
    DateTimeOffset Timestamp,
    double WindSpeed,
    double WindDirection,
    double AmbientTemperature,
    double RotorSpeed,
    double PowerOutput,
    double NacelleDirection,
    double BladePitch,
    double GeneratorTemp,
    double GearboxTemp,
    double Vibration,
    string Status);