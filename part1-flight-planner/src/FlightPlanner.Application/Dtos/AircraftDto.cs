namespace FlightPlanner.Application.Dtos;

public record AircraftDto(int Id, string Name, double CruiseSpeedKmh, double FuelBurnPerHourKg, double TakeoffFuelKg);
