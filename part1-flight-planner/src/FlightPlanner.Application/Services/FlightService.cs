using FlightPlanner.Application.Calculators;
using FlightPlanner.Application.Dtos;
using FlightPlanner.Application.Exceptions;
using FlightPlanner.Application.Repositories;
using FlightPlanner.Domain;
using Microsoft.Extensions.Logging;

namespace FlightPlanner.Application.Services;

public class FlightService : IFlightService
{
    private readonly IFlightRepository _flightRepository;
    private readonly IAirportRepository _airportRepository;
    private readonly IAircraftRepository _aircraftRepository;
    private readonly IDistanceCalculator _distanceCalculator;
    private readonly IFuelCalculator _fuelCalculator;
    private readonly ILogger<FlightService> _logger;

    public FlightService(
        IFlightRepository flightRepository,
        IAirportRepository airportRepository,
        IAircraftRepository aircraftRepository,
        IDistanceCalculator distanceCalculator,
        IFuelCalculator fuelCalculator,
        ILogger<FlightService> logger)
    {
        _flightRepository = flightRepository;
        _airportRepository = airportRepository;
        _aircraftRepository = aircraftRepository;
        _distanceCalculator = distanceCalculator;
        _fuelCalculator = fuelCalculator;
        _logger = logger;
    }

    public async Task<IReadOnlyList<FlightDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var flights = await _flightRepository.GetAllAsync(cancellationToken);
        return flights.Select(ToDto).ToList();
    }

    public async Task<FlightDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var flight = await _flightRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Flight {id} was not found.");
        return ToDto(flight);
    }

    public async Task<FlightDto> CreateAsync(CreateFlightRequest request, CancellationToken cancellationToken = default)
    {
        var (departure, destination, aircraft) = await ResolveAndValidateAsync(
            request.DepartureAirportId, request.DestinationAirportId, request.AircraftId, cancellationToken);

        var now = DateTime.UtcNow;
        var flight = new Flight
        {
            DepartureAirportId = departure.Id,
            DestinationAirportId = destination.Id,
            AircraftId = aircraft.Id,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        ApplyCalculations(flight, departure, destination, aircraft);

        await _flightRepository.AddAsync(flight, cancellationToken);

        _logger.LogInformation(
            "Created flight {FlightId}: {Departure} -> {Destination} on {Aircraft} ({DistanceKm:F0} km, {FuelKg:F0} kg fuel)",
            flight.Id, departure.IcaoCode, destination.IcaoCode, aircraft.Name, flight.DistanceKm, flight.EstimatedFuelKg);

        return ToDto(flight, departure, destination, aircraft);
    }

    public async Task<FlightDto> UpdateAsync(UpdateFlightRequest request, CancellationToken cancellationToken = default)
    {
        var flight = await _flightRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Flight {request.Id} was not found.");

        var (departure, destination, aircraft) = await ResolveAndValidateAsync(
            request.DepartureAirportId, request.DestinationAirportId, request.AircraftId, cancellationToken);

        flight.DepartureAirportId = departure.Id;
        flight.DestinationAirportId = destination.Id;
        flight.AircraftId = aircraft.Id;
        flight.UpdatedAtUtc = DateTime.UtcNow;

        ApplyCalculations(flight, departure, destination, aircraft);

        await _flightRepository.UpdateAsync(flight, cancellationToken);

        _logger.LogInformation(
            "Updated flight {FlightId}: {Departure} -> {Destination} on {Aircraft} ({DistanceKm:F0} km, {FuelKg:F0} kg fuel)",
            flight.Id, departure.IcaoCode, destination.IcaoCode, aircraft.Name, flight.DistanceKm, flight.EstimatedFuelKg);

        return ToDto(flight, departure, destination, aircraft);
    }

    public async Task<IReadOnlyList<AirportDto>> GetAvailableAirportsAsync(CancellationToken cancellationToken = default)
    {
        var airports = await _airportRepository.GetAllAsync(cancellationToken);
        return airports.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<AircraftDto>> GetAvailableAircraftAsync(CancellationToken cancellationToken = default)
    {
        var aircraft = await _aircraftRepository.GetAllAsync(cancellationToken);
        return aircraft.Select(ToDto).ToList();
    }

    private async Task<(Airport Departure, Airport Destination, Aircraft Aircraft)> ResolveAndValidateAsync(
        int departureAirportId, int destinationAirportId, int aircraftId, CancellationToken cancellationToken)
    {
        if (departureAirportId == destinationAirportId)
        {
            _logger.LogWarning("Rejected flight request: departure and destination airport are the same ({AirportId})", departureAirportId);
            throw new ValidationException("Departure and destination airport must be different.");
        }

        var departure = await _airportRepository.GetByIdAsync(departureAirportId, cancellationToken)
            ?? throw new NotFoundException($"Departure airport {departureAirportId} was not found.");
        var destination = await _airportRepository.GetByIdAsync(destinationAirportId, cancellationToken)
            ?? throw new NotFoundException($"Destination airport {destinationAirportId} was not found.");
        var aircraft = await _aircraftRepository.GetByIdAsync(aircraftId, cancellationToken)
            ?? throw new NotFoundException($"Aircraft {aircraftId} was not found.");

        return (departure, destination, aircraft);
    }

    private void ApplyCalculations(Flight flight, Airport departure, Airport destination, Aircraft aircraft)
    {
        flight.DistanceKm = _distanceCalculator.CalculateDistanceKm(
            departure.Latitude, departure.Longitude, destination.Latitude, destination.Longitude);
        flight.EstimatedFuelKg = _fuelCalculator.CalculateFuelKg(aircraft, flight.DistanceKm);
        flight.EstimatedFlightTimeHours = _fuelCalculator.CalculateFlightTimeHours(aircraft, flight.DistanceKm);
    }

    private static FlightDto ToDto(Flight flight)
    {
        if (flight.DepartureAirport is null || flight.DestinationAirport is null || flight.Aircraft is null)
        {
            throw new InvalidOperationException(
                $"Flight {flight.Id} was loaded without its related Airport/Aircraft navigation properties.");
        }

        return ToDto(flight, flight.DepartureAirport, flight.DestinationAirport, flight.Aircraft);
    }

    private static FlightDto ToDto(Flight flight, Airport departure, Airport destination, Aircraft aircraft) => new(
        flight.Id,
        ToDto(departure),
        ToDto(destination),
        ToDto(aircraft),
        flight.DistanceKm,
        flight.EstimatedFuelKg,
        flight.EstimatedFlightTimeHours,
        flight.CreatedAtUtc,
        flight.UpdatedAtUtc);

    private static AirportDto ToDto(Airport airport) => new(
        airport.Id, airport.IcaoCode, airport.IataCode, airport.Name, airport.City, airport.Country, airport.Latitude, airport.Longitude);

    private static AircraftDto ToDto(Aircraft aircraft) => new(
        aircraft.Id, aircraft.Name, aircraft.CruiseSpeedKmh, aircraft.FuelBurnPerHourKg, aircraft.TakeoffFuelKg);
}
