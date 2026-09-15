using FlightPlanner.Domain;

namespace FlightPlanner.Application.Calculators;

public interface IFuelCalculator
{
    double CalculateFuelKg(Aircraft aircraft, double distanceKm);
    double CalculateFlightTimeHours(Aircraft aircraft, double distanceKm);
}
