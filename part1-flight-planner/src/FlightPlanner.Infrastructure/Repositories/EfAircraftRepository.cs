using FlightPlanner.Application.Repositories;
using FlightPlanner.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Infrastructure.Repositories;

public class EfAircraftRepository : IAircraftRepository
{
    private readonly FlightPlannerDbContext _dbContext;

    public EfAircraftRepository(FlightPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Aircraft>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Aircraft.AsNoTracking().OrderBy(a => a.Name).ToListAsync(cancellationToken);

    public Task<Aircraft?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _dbContext.Aircraft.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}
