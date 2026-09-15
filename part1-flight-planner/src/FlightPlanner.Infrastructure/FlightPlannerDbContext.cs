using FlightPlanner.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Infrastructure;

public class FlightPlannerDbContext : DbContext
{
    public FlightPlannerDbContext(DbContextOptions<FlightPlannerDbContext> options) : base(options)
    {
    }

    public DbSet<Airport> Airports => Set<Airport>();
    public DbSet<Aircraft> Aircraft => Set<Aircraft>();
    public DbSet<Flight> Flights => Set<Flight>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasIndex(a => a.IcaoCode).IsUnique();
            entity.Property(a => a.IcaoCode).HasMaxLength(4).IsRequired();
            entity.Property(a => a.IataCode).HasMaxLength(3).IsRequired();
            entity.Property(a => a.Name).IsRequired();
            entity.Property(a => a.City).IsRequired();
            entity.Property(a => a.Country).IsRequired();
        });

        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.Property(a => a.Name).IsRequired();
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasOne(f => f.DepartureAirport)
                .WithMany()
                .HasForeignKey(f => f.DepartureAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.DestinationAirport)
                .WithMany()
                .HasForeignKey(f => f.DestinationAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.Aircraft)
                .WithMany()
                .HasForeignKey(f => f.AircraftId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
