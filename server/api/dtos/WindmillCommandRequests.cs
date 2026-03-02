namespace api.Dtos;

// base request 
public record WindmillCommandRequest(string Action);

// setInterval
public record SetIntervalCommandRequest(string Action, int Value) : WindmillCommandRequest(Action);

// stop
public record StopCommandRequest(string Action, string? Reason) : WindmillCommandRequest(Action);

// start
public record StartCommandRequest(string Action) : WindmillCommandRequest(Action);

// setPitch
public record SetPitchCommandRequest(string Action, double Angle) : WindmillCommandRequest(Action);