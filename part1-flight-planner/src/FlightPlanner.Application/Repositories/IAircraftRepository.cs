using FlightPlanner.Domain;

namespace FlightPlanner.Application.Repositories;

public interface IAircraftRepository
{
    Task<IReadOnlyList<Aircraft>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Aircraft?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
