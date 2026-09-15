using FlightPlanner.Application.Dtos;
using FlightPlanner.Application.Exceptions;
using FlightPlanner.Application.Services;
using FlightPlanner.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Web.Controllers;

public class FlightsController : Controller
{
    private readonly IFlightService _flightService;
    private readonly ILogger<FlightsController> _logger;

    public FlightsController(IFlightService flightService, ILogger<FlightsController> logger)
    {
        _flightService = flightService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var flights = await _flightService.GetAllAsync(cancellationToken);
        return View(flights);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        return View(await BuildFormViewModelAsync(new FlightFormViewModel(), cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FlightFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(await BuildFormViewModelAsync(model, cancellationToken));
        }

        try
        {
            await _flightService.CreateAsync(
                new CreateFlightRequest
                {
                    DepartureAirportId = model.DepartureAirportId,
                    DestinationAirportId = model.DestinationAirportId,
                    AircraftId = model.AircraftId,
                },
                cancellationToken);

            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(await BuildFormViewModelAsync(model, cancellationToken));
        }
        catch (NotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(await BuildFormViewModelAsync(model, cancellationToken));
        }
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        try
        {
            var flight = await _flightService.GetByIdAsync(id, cancellationToken);
            var model = new FlightFormViewModel
            {
                Id = flight.Id,
                DepartureAirportId = flight.DepartureAirport.Id,
                DestinationAirportId = flight.DestinationAirport.Id,
                AircraftId = flight.Aircraft.Id,
            };
            return View(await BuildFormViewModelAsync(model, cancellationToken));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FlightFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(await BuildFormViewModelAsync(model, cancellationToken));
        }

        try
        {
            await _flightService.UpdateAsync(
                new UpdateFlightRequest
                {
                    Id = model.Id,
                    DepartureAirportId = model.DepartureAirportId,
                    DestinationAirportId = model.DestinationAirportId,
                    AircraftId = model.AircraftId,
                },
                cancellationToken);

            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(await BuildFormViewModelAsync(model, cancellationToken));
        }
        catch (NotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(await BuildFormViewModelAsync(model, cancellationToken));
        }
    }

    private async Task<FlightFormViewModel> BuildFormViewModelAsync(FlightFormViewModel model, CancellationToken cancellationToken)
    {
        var airports = await _flightService.GetAvailableAirportsAsync(cancellationToken);
        var aircraft = await _flightService.GetAvailableAircraftAsync(cancellationToken);

        model.AirportOptions = FlightFormViewModel.BuildAirportOptions(airports);
        model.AircraftOptions = FlightFormViewModel.BuildAircraftOptions(aircraft);
        return model;
    }
}
