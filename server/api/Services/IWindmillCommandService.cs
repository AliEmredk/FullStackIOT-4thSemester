using System.Text.Json;

namespace api.Services;

public interface IWindmillCommandService
{
    Task SendCommandAsync(string turbineId, JsonElement commandJson, int userId);
}