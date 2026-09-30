namespace TinyDrone.TinyDrone.WebApi.Models;

public record DroneDTO(string Name, double Speed, double X, double Y, double Z, string? Status);