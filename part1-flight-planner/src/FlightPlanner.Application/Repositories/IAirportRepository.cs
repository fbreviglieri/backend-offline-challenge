using FlightPlanner.Domain;

namespace FlightPlanner.Application.Repositories;

public interface IAirportRepository
{
    Task<IReadOnlyList<Airport>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Airport?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
