using FlightPlanner.Domain;

namespace FlightPlanner.Application.Repositories;

public interface IFlightRepository
{
    Task<IReadOnlyList<Flight>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Flight flight, CancellationToken cancellationToken = default);
    Task UpdateAsync(Flight flight, CancellationToken cancellationToken = default);
}
