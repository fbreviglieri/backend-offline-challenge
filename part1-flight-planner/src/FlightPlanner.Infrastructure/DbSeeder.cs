using System.Text.Json;
using FlightPlanner.Domain;
using Microsoft.Extensions.Logging;

namespace FlightPlanner.Infrastructure;

/// <summary>
/// Seeds the airport and aircraft reference data from the static JSON files under SeedData/
/// on first run. Kept deliberately simple/offline: no external API calls, no network dependency.
/// </summary>
public class DbSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly FlightPlannerDbContext _dbContext;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(FlightPlannerDbContext dbContext, ILogger<DbSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_dbContext.Airports.Any())
        {
            var airports = await LoadAsync<Airport>("airports.json", cancellationToken);
            _dbContext.Airports.AddRange(airports);
            _logger.LogInformation("Seeded {Count} airports", airports.Count);
        }

        if (!_dbContext.Aircraft.Any())
        {
            var aircraft = await LoadAsync<Aircraft>("aircraft.json", cancellationToken);
            _dbContext.Aircraft.AddRange(aircraft);
            _logger.LogInformation("Seeded {Count} aircraft", aircraft.Count);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<List<T>> LoadAsync<T>(string fileName, CancellationToken cancellationToken)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", fileName);
        await using var stream = File.OpenRead(path);
        var items = await JsonSerializer.DeserializeAsync<List<T>>(stream, JsonOptions, cancellationToken);
        return items ?? throw new InvalidOperationException($"Seed file '{fileName}' deserialized to null.");
    }
}
