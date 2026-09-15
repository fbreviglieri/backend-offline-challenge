using FlightPlanner.Domain;

namespace FlightPlanner.Application.Calculators;

/// <summary>
/// Fuel = fixed takeoff/taxi effort + cruise consumption over the flight time
/// (distance / cruise speed) at the aircraft's per-hour burn rate.
/// </summary>
public class FuelCalculator : IFuelCalculator
{
    public double CalculateFuelKg(Aircraft aircraft, double distanceKm)
    {
        var flightTimeHours = CalculateFlightTimeHours(aircraft, distanceKm);
        return aircraft.TakeoffFuelKg + flightTimeHours * aircraft.FuelBurnPerHourKg;
    }

    public double CalculateFlightTimeHours(Aircraft aircraft, double distanceKm)
    {
        if (aircraft.CruiseSpeedKmh <= 0)
        {
            throw new ArgumentException("Aircraft cruise speed must be greater than zero.", nameof(aircraft));
        }

        return distanceKm / aircraft.CruiseSpeedKmh;
    }
}
