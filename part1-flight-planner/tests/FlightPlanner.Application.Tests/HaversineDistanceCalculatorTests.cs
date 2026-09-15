using FlightPlanner.Application.Calculators;
using Xunit;

namespace FlightPlanner.Application.Tests;

public class HaversineDistanceCalculatorTests
{
    private readonly HaversineDistanceCalculator _calculator = new();

    [Fact]
    public void CalculateDistanceKm_SamePoint_ReturnsZero()
    {
        var distance = _calculator.CalculateDistanceKm(40.6413, -73.7781, 40.6413, -73.7781);

        Assert.Equal(0, distance, precision: 6);
    }

    [Fact]
    public void CalculateDistanceKm_JfkToLhr_ReturnsKnownApproximateDistance()
    {
        // JFK (New York) to LHR (London): commonly cited great-circle distance is ~5540-5560 km.
        var distance = _calculator.CalculateDistanceKm(40.6413, -73.7781, 51.4700, -0.4543);

        Assert.InRange(distance, 5490, 5610);
    }

    [Fact]
    public void CalculateDistanceKm_IsSymmetric()
    {
        var forward = _calculator.CalculateDistanceKm(40.6413, -73.7781, 51.4700, -0.4543);
        var backward = _calculator.CalculateDistanceKm(51.4700, -0.4543, 40.6413, -73.7781);

        Assert.Equal(forward, backward, precision: 6);
    }
}
