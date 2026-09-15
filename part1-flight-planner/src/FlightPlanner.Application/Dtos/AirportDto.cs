namespace FlightPlanner.Application.Dtos;

public record AirportDto(int Id, string IcaoCode, string IataCode, string Name, string City, string Country, double Latitude, double Longitude);
