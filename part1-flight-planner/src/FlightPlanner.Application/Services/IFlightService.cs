using FlightPlanner.Application.Dtos;

namespace FlightPlanner.Application.Services;

public interface IFlightService
{
    Task<IReadOnlyList<FlightDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<FlightDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FlightDto> CreateAsync(CreateFlightRequest request, CancellationToken cancellationToken = default);
    Task<FlightDto> UpdateAsync(UpdateFlightRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AirportDto>> GetAvailableAirportsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AircraftDto>> GetAvailableAircraftAsync(CancellationToken cancellationToken = default);
}
