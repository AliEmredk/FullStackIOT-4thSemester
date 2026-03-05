using api.dtos;

namespace api.Services;

public interface IWindmillTelemetryService
{
    Task SaveTelemetryAsync(WindmillTelemetryDto dto, CancellationToken ct = default);
    Task SaveAlertAsync(WindmillAlertDto dto, CancellationToken ct = default);
}