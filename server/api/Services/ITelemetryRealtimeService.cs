using api.dtos;
using StateleSSE.AspNetCore.EfRealtime;

namespace api.Services;

public interface ITelemetryRealtimeService
{
    Task<RealtimeListenResponse<List<WindmillTelemetryDto>>> GetTelemetryAsync(
        string connectionId,
        string turbineId,
        int minutesBack = 60,
        int maxPoints = 1000);
}