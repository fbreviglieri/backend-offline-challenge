using FlightPlanner.Application.Dtos;

namespace FlightPlanner.Web.Models;

public class FlightReportViewModel
{
    public IReadOnlyList<FlightDto> Flights { get; set; } = Array.Empty<FlightDto>();
    public double TotalDistanceKm { get; set; }
    public double TotalFuelKg { get; set; }
    public double TotalFlightTimeHours { get; set; }
}
