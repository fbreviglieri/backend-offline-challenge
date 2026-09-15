using FlightPlanner.Application.Repositories;
using FlightPlanner.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Infrastructure.Repositories;

public class EfFlightRepository : IFlightRepository
{
    private readonly FlightPlannerDbContext _dbContext;

    public EfFlightRepository(FlightPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Flight>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Include(_dbContext.Flights.AsNoTracking())
            .OrderByDescending(f => f.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        Include(_dbContext.Flights).FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task AddAsync(Flight flight, CancellationToken cancellationToken = default)
    {
        _dbContext.Flights.Add(flight);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await LoadNavigationPropertiesAsync(flight, cancellationToken);
    }

    public async Task UpdateAsync(Flight flight, CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
        await LoadNavigationPropertiesAsync(flight, cancellationToken);
    }

    private async Task LoadNavigationPropertiesAsync(Flight flight, CancellationToken cancellationToken)
    {
        await _dbContext.Entry(flight).Reference(f => f.DepartureAirport).LoadAsync(cancellationToken);
        await _dbContext.Entry(flight).Reference(f => f.DestinationAirport).LoadAsync(cancellationToken);
        await _dbContext.Entry(flight).Reference(f => f.Aircraft).LoadAsync(cancellationToken);
    }

    private static IQueryable<Flight> Include(IQueryable<Flight> query) =>
        query.Include(f => f.DepartureAirport)
             .Include(f => f.DestinationAirport)
             .Include(f => f.Aircraft);
}
