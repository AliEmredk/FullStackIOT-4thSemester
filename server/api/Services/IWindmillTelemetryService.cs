using api.dtos;

namespace api.Services;

public interface IWindmillTelemetryService
{
    Task SaveTelemetryAsync(WindmillTelemetryDto dto, CancellationToken ct = default);
    Task SaveAlertAsync(WindmillAlertDto dto, CancellationToken ct = default);
    Task<List<WindmillTelemetryDto>> GetLatestAsync(CancellationToken ct = default);
    Task<List<WindmillTelemetryDto>> GetHistoryAsync(
        string turbineId,
        DateTimeOffset from,
        DateTimeOffset to,
        int maxPoints = 2000,
        CancellationToken ct = default);
}