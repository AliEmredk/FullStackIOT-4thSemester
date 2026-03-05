namespace api.dtos;

public class WindmillAlertDto
{
    public string FarmId { get; set; } = "";
    public string TurbineId { get; set; } = "";
    public string TurbineName { get; set; } = "";
    
    public DateTimeOffset? Timestamp { get; set; }

    // Info/Warning/Critical
    public string Severity { get; set; } = "Warning";

    public string Message { get; set; } = "";
}