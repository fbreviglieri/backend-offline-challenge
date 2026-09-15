namespace FlightPlanner.Domain;

public class Flight
{
    public int Id { get; set; }

    public int DepartureAirportId { get; set; }
    public Airport? DepartureAirport { get; set; }

    public int DestinationAirportId { get; set; }
    public Airport? DestinationAirport { get; set; }

    public int AircraftId { get; set; }
    public Aircraft? Aircraft { get; set; }

    public double DistanceKm { get; set; }
    public double EstimatedFuelKg { get; set; }
    public double EstimatedFlightTimeHours { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
