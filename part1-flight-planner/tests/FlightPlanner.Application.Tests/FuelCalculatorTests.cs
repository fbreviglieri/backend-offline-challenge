using FlightPlanner.Application.Calculators;
using FlightPlanner.Domain;
using Xunit;

namespace FlightPlanner.Application.Tests;

public class FuelCalculatorTests
{
    private readonly FuelCalculator _calculator = new();

    private static Aircraft SampleAircraft() => new()
    {
        Name = "Test Aircraft",
        CruiseSpeedKmh = 500,
        FuelBurnPerHourKg = 1000,
        TakeoffFuelKg = 100,
    };

    [Fact]
    public void CalculateFuelKg_ZeroDistance_ReturnsOnlyTakeoffFuel()
    {
        var aircraft = SampleAircraft();

        var fuel = _calculator.CalculateFuelKg(aircraft, distanceKm: 0);

        Assert.Equal(aircraft.TakeoffFuelKg, fuel, precision: 6);
    }

    [Fact]
    public void CalculateFuelKg_AddsCruiseConsumptionToTakeoffFuel()
    {
        var aircraft = SampleAircraft();

        // 1000 km at 500 km/h => 2 hours cruise => 2 * 1000 kg/h = 2000 kg + 100 kg takeoff = 2100 kg
        var fuel = _calculator.CalculateFuelKg(aircraft, distanceKm: 1000);

        Assert.Equal(2100, fuel, precision: 6);
    }

    [Fact]
    public void CalculateFlightTimeHours_ComputesDistanceOverCruiseSpeed()
    {
        var aircraft = SampleAircraft();

        var flightTime = _calculator.CalculateFlightTimeHours(aircraft, distanceKm: 1000);

        Assert.Equal(2, flightTime, precision: 6);
    }

    [Fact]
    public void CalculateFuelKg_ZeroCruiseSpeed_Throws()
    {
        var aircraft = SampleAircraft();
        aircraft.CruiseSpeedKmh = 0;

        Assert.Throws<ArgumentException>(() => _calculator.CalculateFuelKg(aircraft, distanceKm: 100));
    }
}
