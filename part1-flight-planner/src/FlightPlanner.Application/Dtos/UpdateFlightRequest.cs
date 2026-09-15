using System.ComponentModel.DataAnnotations;

namespace FlightPlanner.Application.Dtos;

public class UpdateFlightRequest
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int DepartureAirportId { get; set; }

    [Required]
    public int DestinationAirportId { get; set; }

    [Required]
    public int AircraftId { get; set; }
}
