using FlightPlanner.Application.Repositories;
using FlightPlanner.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Infrastructure.Repositories;

public class EfAirportRepository : IAirportRepository
{
    private readonly FlightPlannerDbContext _dbContext;

    public EfAirportRepository(FlightPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Airport>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Airports.AsNoTracking().OrderBy(a => a.Name).ToListAsync(cancellationToken);

    public Task<Airport?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _dbContext.Airports.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}
