using System.ComponentModel.DataAnnotations;
using FlightPlanner.Application.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightPlanner.Web.Models;

public class FlightFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please select a departure airport.")]
    [Display(Name = "Departure airport")]
    public int DepartureAirportId { get; set; }

    [Required(ErrorMessage = "Please select a destination airport.")]
    [Display(Name = "Destination airport")]
    public int DestinationAirportId { get; set; }

    [Required(ErrorMessage = "Please select an aircraft.")]
    [Display(Name = "Aircraft")]
    public int AircraftId { get; set; }

    public IEnumerable<SelectListItem> AirportOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> AircraftOptions { get; set; } = Enumerable.Empty<SelectListItem>();

    public static IEnumerable<SelectListItem> BuildAirportOptions(IReadOnlyList<AirportDto> airports) =>
        airports.Select(a => new SelectListItem($"{a.IataCode} - {a.Name} ({a.City}, {a.Country})", a.Id.ToString()));

    public static IEnumerable<SelectListItem> BuildAircraftOptions(IReadOnlyList<AircraftDto> aircraft) =>
        aircraft.Select(a => new SelectListItem(a.Name, a.Id.ToString()));
}
