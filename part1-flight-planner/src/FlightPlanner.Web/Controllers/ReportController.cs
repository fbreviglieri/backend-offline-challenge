using FlightPlanner.Application.Services;
using FlightPlanner.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Web.Controllers;

public class ReportController : Controller
{
    private readonly IFlightService _flightService;

    public ReportController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var flights = await _flightService.GetAllAsync(cancellationToken);

        var model = new FlightReportViewModel
        {
            Flights = flights,
            TotalDistanceKm = flights.Sum(f => f.DistanceKm),
            TotalFuelKg = flights.Sum(f => f.EstimatedFuelKg),
            TotalFlightTimeHours = flights.Sum(f => f.EstimatedFlightTimeHours),
        };

        return View(model);
    }
}
