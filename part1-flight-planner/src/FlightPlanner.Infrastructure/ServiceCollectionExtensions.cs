using FlightPlanner.Application.Repositories;
using FlightPlanner.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FlightPlanner.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFlightPlannerInfrastructure(this IServiceCollection services, string sqliteConnectionString)
    {
        services.AddDbContext<FlightPlannerDbContext>(options => options.UseSqlite(sqliteConnectionString));

        services.AddScoped<IAirportRepository, EfAirportRepository>();
        services.AddScoped<IAircraftRepository, EfAircraftRepository>();
        services.AddScoped<IFlightRepository, EfFlightRepository>();
        services.AddScoped<DbSeeder>();

        return services;
    }
}
