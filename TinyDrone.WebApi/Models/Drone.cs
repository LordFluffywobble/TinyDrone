using TinyDrone.TinyDrone.WebApi;

namespace TinyDrone.TinyDrone.WebApi.Models;

public record Drone (int Id, string Name,double Speed, double X, double Y, double Z, string? Status);

