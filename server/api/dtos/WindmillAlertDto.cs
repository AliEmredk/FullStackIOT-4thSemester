namespace api.dtos;

public record WindmillAlertDto(
    string TurbineId,
    string FarmId,
    DateTimeOffset Timestamp,
    string Severity,
    string Message);