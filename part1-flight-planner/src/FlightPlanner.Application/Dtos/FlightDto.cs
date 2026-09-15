namespace FlightPlanner.Application.Dtos;

public record FlightDto(
    int Id,
    AirportDto DepartureAirport,
    AirportDto DestinationAirport,
    AircraftDto Aircraft,
    double DistanceKm,
    double EstimatedFuelKg,
    double EstimatedFlightTimeHours,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
